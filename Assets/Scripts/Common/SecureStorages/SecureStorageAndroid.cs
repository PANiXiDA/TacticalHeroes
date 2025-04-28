using System;
using System.Text;

using Assets.Scripts.Common.SecureStorages.Extensions;

using UnityEngine;

namespace Assets.Scripts.Common.SecureStorages
{
    public sealed class SecureStorageAndroid : ISecureStorage
    {
        private const string Alias = "MyAppKeystoreAlias";
        private const string RsaAlgo = "RSA/ECB/PKCS1Padding";

        public void SaveData(string key, string value)
        {
            try
            {
                var enc = Encrypt(value);
                PlayerPrefs.SetString(key, Convert.ToBase64String(enc));
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SecureStorageAndroid] Save error: {e}");
            }
        }

        public string LoadData(string key)
        {
            try
            {
                if (!PlayerPrefs.HasKey(key))
                    return null;

                var enc = Convert.FromBase64String(PlayerPrefs.GetString(key));
                return Decrypt(enc);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SecureStorageAndroid] Load error: {e}");
                return null;
            }
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

        private byte[] Encrypt(string plain)
        {
            var keyStore = GetKeyStore();
            var entry = keyStore.Call<AndroidJavaObject>("getEntry", Alias, null);
            if (entry == null)
            {
                GenerateKey();
                entry = keyStore.Call<AndroidJavaObject>("getEntry", Alias, null);
            }

            var pubKey = entry
                .Call<AndroidJavaObject>("getCertificate")
                .Call<AndroidJavaObject>("getPublicKey");

            var cipher = new AndroidJavaClass("javax.crypto.Cipher").CallStatic<AndroidJavaObject>("getInstance", RsaAlgo);

            cipher.Call("init", 1, pubKey);

            var data = Encoding.UTF8.GetBytes(plain);
            return cipher.Call<byte[]>("doFinal", data);
        }

        private string Decrypt(byte[] encrypted)
        {
            var keyStore = GetKeyStore();
            var entry = keyStore.Call<AndroidJavaObject>("getEntry", Alias, null);
            var privKey = entry.Call<AndroidJavaObject>("getPrivateKey");

            var cipher = new AndroidJavaClass("javax.crypto.Cipher").CallStatic<AndroidJavaObject>("getInstance", RsaAlgo);

            cipher.Call("init", 2, privKey);
            var dec = cipher.Call<byte[]>("doFinal", encrypted);
            return Encoding.UTF8.GetString(dec);
        }

        private void GenerateKey()
        {
            int sdk = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");

            if (sdk >= 23)
            {
                GenerateKeyApi23Plus();
            }
            else
            {
                GenerateKeyApi18_22();
            }
        }

        private void GenerateKeyApi23Plus()
        {
            var kpg = new AndroidJavaClass("java.security.KeyPairGenerator").CallStatic<AndroidJavaObject>("getInstance", "RSA", "AndroidKeyStore");

            var builder = new AndroidJavaObject(
                "android.security.keystore.KeyGenParameterSpec$Builder",
                Alias,
                3
            )
            .Call<AndroidJavaObject>("setKeySize", 2048)
            .Call<AndroidJavaObject>("setBlockModes", (object)new[] { "ECB" })
            .Call<AndroidJavaObject>("setEncryptionPaddings", (object)new[] { "PKCS1Padding" });

            var spec = builder.Call<AndroidJavaObject>("build");
            kpg.Call("initialize", spec);

            kpg.Call<AndroidJavaObject>("generateKeyPair");
        }

        private void GenerateKeyApi18_22()
        {
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

            var calendar = new AndroidJavaObject("java.util.GregorianCalendar");
            var start = calendar.Call<AndroidJavaObject>("getTime");
            calendar.Call("add", new AndroidJavaClass("java.util.Calendar").GetStatic<int>("YEAR"), 25);
            var end = calendar.Call<AndroidJavaObject>("getTime");

            var builder = new AndroidJavaObject(
                "android.security.KeyPairGeneratorSpec$Builder",
                activity
            )
            .Call<AndroidJavaObject>("setAlias", Alias)
            .Call<AndroidJavaObject>("setSerialNumber", new AndroidJavaObject("java.math.BigInteger", "1"))
            .Call<AndroidJavaObject>("setSubject", new AndroidJavaObject("javax.security.auth.x500.X500Principal",
                                                           $"CN={Alias}"))
            .Call<AndroidJavaObject>("setStartDate", start)
            .Call<AndroidJavaObject>("setEndDate", end)
            .Call<AndroidJavaObject>("setKeySize", 2048);

            var spec = builder.Call<AndroidJavaObject>("build");

            var kpg = new AndroidJavaClass("java.security.KeyPairGenerator").CallStatic<AndroidJavaObject>("getInstance", "RSA", "AndroidKeyStore");

            kpg.Call("initialize", spec);
            kpg.Call<AndroidJavaObject>("generateKeyPair");
        }

        private static AndroidJavaObject GetKeyStore()
        {
            var ks = new AndroidJavaClass("java.security.KeyStore").CallStatic<AndroidJavaObject>("getInstance", "AndroidKeyStore");
            ks.Call("load", (AndroidJavaObject)null);
            return ks;
        }
    }
}
