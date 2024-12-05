using System;

namespace Assets.Scripts.Common.SecureStorages.Extensions
{
    public static class SecureStorageFactory
    {
        public static ISecureStorage Create()
        {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
            return new SecureStorageWindowAndLinux();
#elif UNITY_ANDROID
            return new SecureStorageAndroid();
#elif UNITY_IOS
            return new SecureStorageIOS();
#else
            throw new PlatformNotSupportedException("Secure storage is not implemented for this platform.");
#endif
        }
    }

    public interface ISecureStorage
    {
        void SaveData(string key, string value);
        string LoadData(string key);
        void DeleteData(string key);
        void ClearAllData();
    }
}
