using Assets.Scripts.Common.SecureStorages.Extensions;

namespace Assets.Scripts.Common.WebRequest.JWT
{
    public static class JwtTokenManager
    {
        private static readonly ISecureStorage _secureStorage = SecureStorageFactory.Create();
        private const string RefreshTokenKey = "RefreshToken";

        public static string AccessToken { get; set; }

        public static void SaveRefreshToken(string refreshToken)
        {
            _secureStorage.SaveData(RefreshTokenKey, refreshToken);
        }

        public static string LoadRefreshToken()
        {
            return _secureStorage.LoadData(RefreshTokenKey);
        }

        public static void DeleteRefreshToken()
        {
            _secureStorage.DeleteData(RefreshTokenKey);
        }
    }
}
