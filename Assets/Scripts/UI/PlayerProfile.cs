using Assets.Scripts.Infrastructure.Models;
using System;
using UnityEngine;
using Avatar = Assets.Scripts.Infrastructure.Models.Avatar;

namespace Assets.Scripts.UI
{
    public class PlayerProfile : MonoBehaviour
    {
        public static PlayerProfile Instance { get; private set; }

        public string Nickname { get; set; }
        public int Games { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Mmr { get; set; }
        public DateTime LastLogin { get; set; }
        public int Gold { get; set; }
        public bool Premium { get; set; }
        public int AvatarId { get; set; }
        public int FrameId { get; set; }

        public Avatar Avatar { get; set; }
        public Frame Frame { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(
            string nickname,
            int games,
            int wins,
            int loses,
            int mmr,
            DateTime lastLogin,
            int gold,
            bool premium,
            int avatarId,
            int frameId,
            Avatar avatar,
            Frame frame)
        {
            Nickname = nickname;
            Games = games;
            Wins = wins;
            Loses = loses;
            Mmr = mmr;
            LastLogin = lastLogin;
            Gold = gold;
            Premium = premium;
            AvatarId = avatarId;
            FrameId = frameId;
            Avatar = avatar;
            Frame = frame;
        }
    }
}
