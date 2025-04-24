using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SQLite4Unity3d;
using UnityEngine;
using Assets.Scripts.Repositories.Models;
using System.Text.RegularExpressions;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using Assets.Scripts.Common.Constants;

namespace Assets.Scripts.Repositories.Implementations.SQLite.Core
{
    public sealed class SQLiteContext : IDisposable
    {
        public SQLiteConnection Connection { get; private set; }

        public SQLiteContext()
        {
            var dbPath = PrepareSQLiteDbFile();
            Connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

            AutoMigrateSchema();
        }

        public void Dispose()
        {
            Connection?.Close();
            Connection?.Dispose();
        }

        private string PrepareSQLiteDbFile()
        {
            var fileName = DatabasesConstants.SQLiteDatabaseName;
            var outPath = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(outPath))
            {
                return outPath;
            }

            try
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(fileName);
                var txt = handle.WaitForCompletion();
                if (txt != null && txt.bytes != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
                    File.WriteAllBytes(outPath, txt.bytes);
                    Addressables.Release(handle);
                    return outPath;
                }
            }
            catch (InvalidKeyException) { }

            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
            new SQLiteConnection(outPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create).Close();

            return outPath;
        }

        private void AutoMigrateSchema()
        {
            var modelTypes = GetModelTypes()
                .ToDictionary(
                    t => t.GetCustomAttribute<TableAttribute>()?.Name ?? t.Name,
                    t => t);

            var existingTables = Connection
                .Query<TableInfo>("SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
                .Select(t => t.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in modelTypes)
            {
                var tableName = kv.Key;
                var modelType = kv.Value;

                if (!existingTables.Remove(tableName))
                {
                    Connection.CreateTable(modelType, CreateFlags.None);
                }
                else
                {
                    MigrateTableColumns(tableName, modelType);
                }
            }

            foreach (var tableToDrop in existingTables)
            {
                Connection.Execute($"DROP TABLE IF EXISTS {tableToDrop}");
            }
        }

        private void MigrateTableColumns(string tableName, Type modelType)
        {
            var tableSql = Connection.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE type='table' AND name = ?", tableName) ?? "";

            var tableColsInfo = Connection
                .Query<ColumnInfo>($"PRAGMA table_info('{tableName}')")
                .ToList();

            var props = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute<IgnoreAttribute>() == null && !IsCollectionType(p.PropertyType))
                .ToList();

            bool mustRebuild = false;
            foreach (var p in props)
            {
                var colName = p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name;
                var info = tableColsInfo.FirstOrDefault(c => c.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));
                if (info == null)
                {
                    continue;
                }

                var expectedType = GetSqlType(p.PropertyType);
                if (!string.Equals(expectedType, info.Type, StringComparison.OrdinalIgnoreCase))
                {
                    mustRebuild = true;
                    break;
                }

                var isPkModel = p.GetCustomAttribute<PrimaryKeyAttribute>() != null;
                if (isPkModel != (info.Pk == 1))
                {
                    mustRebuild = true;
                    break;
                }

                var isAiModel = p.GetCustomAttribute<AutoIncrementAttribute>() != null;
                var pattern = $"{colName}\\s+\\w+\\s+PRIMARY KEY\\s+AUTOINCREMENT";
                var hasAiInSql = Regex.IsMatch(tableSql, pattern, RegexOptions.IgnoreCase);
                if (isAiModel != hasAiInSql)
                {
                    mustRebuild = true;
                    break;
                }
            }

            if (mustRebuild)
            {
                RebuildTable(tableName, modelType, props, tableColsInfo.Select(c => c.Name).ToList());
                return;
            }

            var modelCols = props
                .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name)
                .ToList();

            var existingCols = tableColsInfo.Select(c => c.Name).ToList();
            foreach (var col in modelCols.Except(existingCols, StringComparer.OrdinalIgnoreCase))
            {
                var p = props.First(x => (x.GetCustomAttribute<ColumnAttribute>()?.Name ?? x.Name) == col);
                var sqlType = GetSqlType(p.PropertyType);
                var defVal = GetDefaultValue(p.PropertyType);
                Connection.Execute($"ALTER TABLE {tableName} ADD COLUMN {col} {sqlType} DEFAULT {defVal}");
            }

            var colsToKeep = modelCols.Intersect(existingCols, StringComparer.OrdinalIgnoreCase).ToList();
            var colsToDrop = existingCols.Except(modelCols, StringComparer.OrdinalIgnoreCase).ToList();
            if (colsToDrop.Any())
            {
                var tmp = $"{tableName}_tmp";
                var keepCsv = string.Join(", ", colsToKeep);
                Connection.Execute($"CREATE TABLE {tmp} AS SELECT {keepCsv} FROM {tableName}");
                Connection.Execute($"DROP TABLE {tableName}");
                Connection.Execute($"ALTER TABLE {tmp} RENAME TO {tableName}");
            }
        }

        private void RebuildTable(
            string tableName,
            Type modelType,
            List<PropertyInfo> props,
            List<string> existingCols)
        {
            var colDefs = props.Select(p =>
            {
                var name = p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name;
                var sqlType = GetSqlType(p.PropertyType);
                var pk = p.GetCustomAttribute<PrimaryKeyAttribute>() != null;
                var ai = p.GetCustomAttribute<AutoIncrementAttribute>() != null;
                var pkPart = pk ? "PRIMARY KEY" + (ai ? " AUTOINCREMENT" : "") : "";
                return $"{name} {sqlType} {pkPart}".Trim();
            });

            var tmpName = tableName + "_tmp";
            var create = $"CREATE TABLE {tmpName} (\n  {string.Join(",\n  ", colDefs)}\n)";
            Connection.Execute(create);

            var copyCols = props
                .Select(p => p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name)
                .Where(c => existingCols.Contains(c, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (copyCols.Any())
            {
                var csv = string.Join(", ", copyCols);
                Connection.Execute($"INSERT INTO {tmpName} ({csv}) SELECT {csv} FROM {tableName}");
            }

            Connection.Execute($"DROP TABLE {tableName}");
            Connection.Execute($"ALTER TABLE {tmpName} RENAME TO {tableName}");
        }

        private static IEnumerable<Type> GetModelTypes()
        {
            var asm = typeof(Unit).Assembly;
            return asm.GetTypes().Where(t => t.IsClass && t.GetCustomAttribute<TableAttribute>() != null);
        }

        private static string GetSqlType(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (t.IsEnum || t == typeof(int) || t == typeof(long)) return "INTEGER";
            if (t == typeof(double) || t == typeof(float)) return "REAL";
            if (t == typeof(bool)) return "INTEGER";
            if (t == typeof(string) || t == typeof(Guid)) return "TEXT";
            if (t == typeof(byte[])) return "BLOB";
            throw new NotSupportedException($"Тип {t.Name} не поддерживается");
        }

        private static string GetDefaultValue(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (t.IsEnum || t == typeof(int) || t == typeof(long)) return "0";
            if (t == typeof(double) || t == typeof(float)) return "0.0";
            if (t == typeof(bool)) return "0";
            if (t == typeof(string) || t == typeof(Guid)) return "''";
            if (t == typeof(byte[])) return "x''";
            return "NULL";
        }

        private static bool IsCollectionType(Type t)
        {
            if (!t.IsGenericType) return false;
            var def = t.GetGenericTypeDefinition();
            return def == typeof(List<>)
                || def == typeof(IEnumerable<>)
                || def == typeof(ICollection<>)
                || def == typeof(IList<>);
        }

        private class TableInfo
        {
            [Column("name")] public string Name { get; set; }
        }

        private class ColumnInfo
        {
            [Column("name")] public string Name { get; set; }
            [Column("type")] public string Type { get; set; }
            [Column("pk")] public int Pk { get; set; }
        }
    }
}
