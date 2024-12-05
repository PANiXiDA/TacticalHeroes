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
        public Avatar Avatar { get; set; }

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
            Avatar avatar)
        {
            Nickname = nickname;
            Games = games;
            Wins = wins;
            Loses = loses;
            Mmr = mmr;
            Avatar = avatar;
        }
    }
}
