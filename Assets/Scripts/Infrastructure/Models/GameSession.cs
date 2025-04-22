using System.Collections.Generic;

namespace Assets.Scripts.Infrastructure.Models
{
    public class GameSession
    {
        public List<PlayerBattleData> Players { get; set; } = new();
    }
}
