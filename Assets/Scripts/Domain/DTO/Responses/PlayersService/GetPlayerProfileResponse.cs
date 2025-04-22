using Assets.Scripts.Infrastructure.Models;
using System;

namespace Assets.Scripts.Infrastructure.Responses.PlayersService
{
    public class GetPlayerProfileResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
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
        public Avatar Avatar { get; set; }
        public Frame Frame { get; set; }
    }
}