using System;

namespace Assets.Scripts.UI.UIAuth.Responses
{
    public class RegistrationResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public bool Confirmed { get; set; }
        public bool Blocked { get; set; }
        public int Role { get; set; }
        public DateTime LastLogin { get; set; }
        public int AvatarId { get; set; }
        public int FrameId { get; set; }
        public int Games { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Mmr { get; set; }
        public int Rank { get; set; }
        public bool Premium { get; set; }
        public int Gold { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
    }
}
