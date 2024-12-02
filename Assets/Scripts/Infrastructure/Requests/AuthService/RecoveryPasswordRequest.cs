namespace Assets.Scripts.Infrastructure.Requests.AuthService
{
    public class RecoveryPasswordRequest
    {
        public string Email { get; set; }

        public RecoveryPasswordRequest(string email)
        {
            Email = email;
        }
    }
}
