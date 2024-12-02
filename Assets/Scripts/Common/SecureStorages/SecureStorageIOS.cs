using Assets.Scripts.Common.SecureStorages.Extensions;
using UnityEngine;

namespace Assets.Scripts.Common.SecureStorages
{
    public class SecureStorageIOS : ISecureStorage
    {
        public void SaveData(string key, string value)
        {
            Debug.Log($"[SecureStorageIOS] Saving data: {key} -> {value}");
        }

        public string LoadData(string key)
        {
            Debug.Log($"[SecureStorageIOS] Loading data for key: {key}");
            return null;
        }

        public void DeleteData(string key)
        {
            Debug.Log($"[SecureStorageIOS] Deleting data for key: {key}");
        }

        public void ClearAllData()
        {
            Debug.Log("[SecureStorageIOS] Clearing all data.");
        }
    }
}
