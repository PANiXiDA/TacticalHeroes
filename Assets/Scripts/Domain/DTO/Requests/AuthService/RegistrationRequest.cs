namespace Assets.Scripts.Infrastructure.Requests.AuthService
{
    public class RegistrationRequest
    {
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }

        public RegistrationRequest(string nickName, string email, string password, string repeatPassword)
        {
            NickName = nickName;
            Email = email;
            Password = password;
            RepeatPassword = repeatPassword;
        }
    }
}
