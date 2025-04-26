#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Assets.Scripts.Common.Constants;

namespace Assets.Scripts.Repositories.Implementations.SQLite.Editor
{
    public class PreBuildCopySQLiteToStreamingAssets : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        private readonly string StreamingFolder = "StreamingAssets";

        public void OnPreprocessBuild(BuildReport report)
        {
            var src = Path.Combine(Application.persistentDataPath, DatabasesConstants.SQLiteDatabaseName);
            if (!File.Exists(src))
            {
                Debug.LogWarning($"[PreBuild] SQLite.db не найден по пути {src}");
                return;
            }

            var targetDir = Path.Combine(Application.dataPath, StreamingFolder, DatabasesConstants.SQLiteDatabasePath);
            Directory.CreateDirectory(targetDir);

            var dst = Path.Combine(targetDir, DatabasesConstants.SQLiteDatabaseName);
            File.Copy(src, dst, true);
            Debug.Log($"[PreBuild] Скопирован SQLite.db → {dst}");

            AssetDatabase.Refresh();
        }
    }
}
#endif
