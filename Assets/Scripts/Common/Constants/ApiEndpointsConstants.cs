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
        public const string LogoutEndpoint = "auth/logout";
        #endregion

        #region PlayersService
        public const string GetPlayerEndpoint = "players";
        public const string UpdatePlayerAvatarEndpoint = "players/update-avatar";
        public const string UpdatePlayerFrameEndpoint = "players/update-frame";
        #endregion

        #region AvatarsService
        public const string GetAvailableAvatarsEndpoint = "avatars/get-available";
        #endregion

        #region FramesService
        public const string GetAvailableFramesEndpoint = "frames/get-available";
        #endregion
    }
}
