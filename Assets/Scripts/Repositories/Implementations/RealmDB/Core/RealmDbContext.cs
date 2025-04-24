using System;
using System.IO;

using Assets.Scripts.Common.Constants;

using Realms;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Repositories.Implementations.RealmDB.Core
{
    public sealed class RealmContext : IDisposable
    {
        public Realm Connection { get; }

        private const int SchemaVersion = 2;

        public RealmContext()
        {
            var dbPath = PrepareRealmFile();

            var config = new RealmConfiguration(dbPath)
            {
                SchemaVersion = SchemaVersion,
                MigrationCallback = OnMigrate
            };
            Connection = Realm.GetInstance(config);
        }

        public void Dispose() => Connection?.Dispose();

        private string PrepareRealmFile()
        {
            var fileName = DatabasesConstants.RealmDbDatabaseName;
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
            using (var r = Realm.GetInstance(new RealmConfiguration(outPath)))

            return outPath;
        }

        private void OnMigrate(Migration migration, ulong oldVer)
        {
            if (oldVer < SchemaVersion)
            {

            }
        }
    }
}
