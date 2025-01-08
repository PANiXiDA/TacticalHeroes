using Assets.Scripts.Infrastructure.Models;

namespace Assets.Scripts.Infrastructure.Responses.MatchmakingeService
{
    public class MatchmakingResponse
    {
        public string GameId { get; set; }
        public int OpponentId { get; set; }
        public string OpponentNickname { get; set; }
        public int Mmr { get; set; }
        public int Rank { get; set; }
        public int Level { get; set; }
        public Avatar Avatar { get; set; }
        public Frame Frame { get; set; }
    }
}
