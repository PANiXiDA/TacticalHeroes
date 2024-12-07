namespace Assets.Scripts.Infrastructure.Requests.AuthService
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; }

        public LogoutRequest(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
