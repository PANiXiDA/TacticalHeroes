namespace Assets.Scripts.Infrastructure.Requests.AuthService
{
    public class ConfirmEmailRequest
    {
        public string Email { get; set; }

        public ConfirmEmailRequest(string email)
        {
            Email = email;
        }
    }
}
