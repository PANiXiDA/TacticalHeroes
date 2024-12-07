namespace Assets.Scripts.Common.Constants
{
    public static class ErrorConstants
    {
        #region AuthErros
        public const string BadRefreshToken = "RefreshToken expired or invalid.";
        public const string ServerUnavailable = "The server is unavailable.";
        public const string IncorrectRepeatPassword = "Passwords must much.";
        public const string NotValidPassword = "The password must contain at least 6 characters, 1 uppercase letter, 1 lowercase letter, and 1 digit.";
        public const string NotValidEmail = "Invalid email format.";
        public const string RequiredNickname = "Nickname is required";
        #endregion
    }
}
