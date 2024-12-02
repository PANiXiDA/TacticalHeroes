using Assets.Scripts.Common.SecureStorages.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Scripts.Common.SecureStorages
{
    public class SecureStorageWindowAndLinux : ISecureStorage
    {
        private string StoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SecureStorage.dat");

        public void SaveData(string key, string value)
        {
            Dictionary<string, byte[]> storage = LoadAllData();

            byte[] encryptedValue = ProtectData(value);
            storage[key] = encryptedValue;

            SaveAllData(storage);
        }

        public string LoadData(string key)
        {
            Dictionary<string, byte[]> storage = LoadAllData();

            if (storage.TryGetValue(key, out byte[] encryptedValue))
            {
                return UnprotectData(encryptedValue);
            }

            return null;
        }

        public void DeleteData(string key)
        {
            Dictionary<string, byte[]> storage = LoadAllData();

            if (storage.Remove(key))
            {
                SaveAllData(storage);
            }
        }

        public void ClearAllData()
        {
            if (File.Exists(StoragePath))
            {
                File.Delete(StoragePath);
            }
        }

        private static byte[] ProtectData(string plainText)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
        }

        private static string UnprotectData(byte[] encryptedData)
        {
            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private Dictionary<string, byte[]> LoadAllData()
        {
            if (!File.Exists(StoragePath))
            {
                return new Dictionary<string, byte[]>();
            }

            using (FileStream fs = new FileStream(StoragePath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, byte[]>)formatter.Deserialize(fs);
            }
        }

        private void SaveAllData(Dictionary<string, byte[]> storage)
        {
            using (FileStream fs = new FileStream(StoragePath, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, storage);
            }
        }
    }
}
