using System;
using UnityEngine;
using System.Text;
using Assets.Scripts.Common.SecureStorages.Extensions;

namespace Assets.Scripts.Common.SecureStorages
{
    public class SecureStorageAndroid : ISecureStorage
    {
        private const string Alias = "MyAppKeystoreAlias";
        private const string AndroidKeyStore = "AndroidKeyStore";

        public void SaveData(string key, string value)
        {
            try
            {
                var encryptedData = Encrypt(value);
                PlayerPrefs.SetString(key, Convert.ToBase64String(encryptedData));
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SecureStorageAndroid] Error saving data: {e.Message}");
            }
        }

        public string LoadData(string key)
        {
            try
            {
                if (PlayerPrefs.HasKey(key))
                {
                    var encryptedData = Convert.FromBase64String(PlayerPrefs.GetString(key));
                    return Decrypt(encryptedData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SecureStorageAndroid] Error loading data: {e.Message}");
            }

            return null;
        }

        public void DeleteData(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }

        public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        private byte[] Encrypt(string plainText)
        {
            try
            {
                var keyStore = new AndroidJavaClass("android.security.keystore.KeyStore");
                keyStore.Call("load", null);

                var privateKeyEntry = keyStore.Call<AndroidJavaObject>("getEntry", Alias, null);
                if (privateKeyEntry == null)
                {
                    GenerateKey();
                    privateKeyEntry = keyStore.Call<AndroidJavaObject>("getEntry", Alias, null);
                }

                var publicKey = privateKeyEntry.Call<AndroidJavaObject>("getCertificate").Call<AndroidJavaObject>("getPublicKey");

                var cipher = new AndroidJavaClass("javax.crypto.Cipher").CallStatic<AndroidJavaObject>("getInstance", "RSA/ECB/PKCS1Padding");
                cipher.Call("init", 1, publicKey);

                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                return cipher.Call<byte[]>("doFinal", plainBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SecureStorageAndroid] Encryption error: {ex.Message}");
                throw;
            }
        }

        private string Decrypt(byte[] encryptedData)
        {
            try
            {
                var keyStore = new AndroidJavaClass("android.security.keystore.KeyStore");
                keyStore.Call("load", null);

                var privateKeyEntry = keyStore.Call<AndroidJavaObject>("getEntry", Alias, null);
                var privateKey = privateKeyEntry.Call<AndroidJavaObject>("getPrivateKey");

                var cipher = new AndroidJavaClass("javax.crypto.Cipher").CallStatic<AndroidJavaObject>("getInstance", "RSA/ECB/PKCS1Padding");
                cipher.Call("init", 2, privateKey);

                var decryptedBytes = cipher.Call<byte[]>("doFinal", encryptedData);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SecureStorageAndroid] Decryption error: {ex.Message}");
                throw;
            }
        }

        private void GenerateKey()
        {
            try
            {
                var keyGenerator = new AndroidJavaClass("java.security.KeyPairGenerator").CallStatic<AndroidJavaObject>("getInstance", "RSA", AndroidKeyStore);
                var keyGenSpec = new AndroidJavaClass("android.security.keystore.KeyGenParameterSpec$Builder")
                    .Call<AndroidJavaObject>("<init>", Alias, 3)
                    .Call<AndroidJavaObject>("setEncryptionPaddings", new object[] { "PKCS1Padding" })
                    .Call<AndroidJavaObject>("build");

                keyGenerator.Call("initialize", keyGenSpec);
                keyGenerator.Call("generateKeyPair");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SecureStorageAndroid] Key generation error: {ex.Message}");
                throw;
            }
        }
    }
}
