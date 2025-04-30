using System;
using System.IO;

using Assets.Scripts.Common.Constants;

using Realms;

using UnityEngine;

namespace Assets.Scripts.Repositories.Implementations.RealmDB.Core
{
    public sealed class RealmContext : IDisposable
    {
        public Realm Connection { get; }

        private const int SchemaVersion = 3;

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
                return outPath;

            var streamPath = Path.Combine(Application.streamingAssetsPath, DatabasesConstants.RealmDbDatabasePath, fileName);

            byte[] data;
            if (Application.platform == RuntimePlatform.Android)
            {
                using var www = UnityEngine.Networking.UnityWebRequest.Get(streamPath);
                www.SendWebRequest();
                while (!www.isDone) { }
                data = www.downloadHandler.data!;
            }
            else
            {
                data = File.ReadAllBytes(streamPath);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
            File.WriteAllBytes(outPath, data);
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
