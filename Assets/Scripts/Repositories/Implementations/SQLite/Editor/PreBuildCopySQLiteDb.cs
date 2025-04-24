#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Assets.Scripts.Common.Constants;

namespace Assets.Scripts.Repositories.Implementations.SQLite.Editor
{
    public class PreBuildCopySQLiteDb : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var srcPath = Path.Combine(Application.persistentDataPath, DatabasesConstants.SQLiteDatabaseName);
            if (!File.Exists(srcPath))
            {
                Debug.LogWarning($"PreBuildCopySQLiteDb: исходной БД не найдено по пути {srcPath}");
                return;
            }

            var projectRoot = Application.dataPath.Replace("Assets", "");
            var projectFolder = Path.Combine(projectRoot, DatabasesConstants.SQLiteDatabasePath);

            Directory.CreateDirectory(projectFolder);
            var projectPath = Path.Combine(projectFolder,DatabasesConstants.SQLiteDatabaseName);

            File.Copy(srcPath, projectPath, overwrite: true);
            Debug.Log($"PreBuildCopySQLiteDb: скопировано {srcPath} → {projectFolder}");

            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null)
            {
                Debug.LogError("PreBuildCopySQLiteDb: AddressableAssetSettings не найдены!");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID($"{projectPath}");
            var entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, settings.DefaultGroup);

            entry.SetAddress(DatabasesConstants.SQLiteDatabaseName);

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true, false);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif