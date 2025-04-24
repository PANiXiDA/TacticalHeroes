#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Assets.Scripts.Common.Constants;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Assets.Scripts.Repositories.Implementations.RealmDB.Editor
{
    public class PreBuildCopyRealmDb : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var srcPath = Path.Combine(Application.persistentDataPath, DatabasesConstants.RealmDbDatabaseName);
            if (!File.Exists(srcPath))
            {
                Debug.LogWarning($"PreBuildCopyRealmDb: исходной БД не найдено по пути {srcPath}");
                return;
            }

            var projectRoot = Application.dataPath.Replace("Assets", "");
            var projectFolder = Path.Combine(projectRoot, DatabasesConstants.RealmDbDatabasePath);

            Directory.CreateDirectory(projectFolder);
            var projectPath = Path.Combine(projectFolder, DatabasesConstants.RealmDbDatabaseName);

            File.Copy(srcPath, projectPath, overwrite: true);
            Debug.Log($"PreBuildCopyRealmDb: скопировано {srcPath} → {projectFolder}");

            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null)
            {
                Debug.LogError("PreBuildCopyRealmDb: AddressableAssetSettings не найдены!");
                return;
            }

            var guid = AssetDatabase.AssetPathToGUID($"{projectPath}");
            var entry = settings.FindAssetEntry(guid) ?? settings.CreateOrMoveEntry(guid, settings.DefaultGroup);

            entry.SetAddress(DatabasesConstants.RealmDbDatabaseName);

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true, false);
            AssetDatabase.SaveAssets();
        }
    }
}

#endif