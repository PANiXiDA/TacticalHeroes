namespace Assets.Scripts.Common.Constants
{
    public static class ApiEndpointsConstants
    {
        #region AuthService
        public const string RegistrationEndpoint = "auth/sign-up";
        public const string LoginEndpoint = "auth/login";
        public const string EmailConfirmEndpoint = "auth/confirm-email";
        public const string RecoveryPasswordEndpoint = "auth/recovery";
        public const string RefreshTokensEndpoint = "auth/refresh";
        #endregion

        #region PlayersService
        public const string GetPlayerEndpoint = "players";
        #endregion
    }
}
