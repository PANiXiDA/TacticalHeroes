#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Assets.Scripts.Common.Constants;

namespace Assets.Scripts.Repositories.Implementations.RealmDB.Editor
{
    public class PreBuildCopyRealmToStreamingAssets : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private readonly string StreamingFolder = "StreamingAssets";

        public void OnPreprocessBuild(BuildReport report)
        {
            var src = Path.Combine(Application.persistentDataPath, DatabasesConstants.RealmDbDatabaseName);
            if (!File.Exists(src))
            {
                Debug.LogWarning($"[PreBuild] RealmDb.realm не найден по пути {src}");
                return;
            }

            var targetDir = Path.Combine(Application.dataPath, StreamingFolder, DatabasesConstants.RealmDbDatabasePath);
            Directory.CreateDirectory(targetDir);

            var dst = Path.Combine(targetDir, DatabasesConstants.RealmDbDatabaseName);
            File.Copy(src, dst, true);
            Debug.Log($"[PreBuild] Скопирован RealmDb.realm → {dst}");

            AssetDatabase.Refresh();
        }
    }
}
#endif
