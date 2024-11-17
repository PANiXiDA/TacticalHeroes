﻿namespace Assets.Scripts.UI.UIAuth.Requests
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
