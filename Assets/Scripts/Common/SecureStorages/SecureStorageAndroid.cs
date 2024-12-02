using Assets.Scripts.Common.SecureStorages.Extensions;
using UnityEngine;

namespace Assets.Scripts.Common.SecureStorages
{
    public class SecureStorageAndroid : ISecureStorage
    {
        public void SaveData(string key, string value)
        {
            Debug.Log($"[SecureStorageAndroid] Saving data: {key} -> {value}");
        }

        public string LoadData(string key)
        {
            Debug.Log($"[SecureStorageAndroid] Loading data for key: {key}");
            return null;
        }

        public void DeleteData(string key)
        {
            Debug.Log($"[SecureStorageAndroid] Deleting data for key: {key}");
        }

        public void ClearAllData()
        {
            Debug.Log("[SecureStorageAndroid] Clearing all data.");
        }
    }
}
