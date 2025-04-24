using System.Collections.Generic;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.DTO.Enums;

namespace Assets.Scripts.Infrastructure.Models
{
    public class GameSession
    {
        public GameState GameState { get; set; }
        public GameType GameType { get; set; }
        public RoundState RoundState { get; set; } = new();
        public List<RoundState> GameHistory { get; set; } = new();
        public List<PlayerBattleData> Players { get; set; } = new();

        public GameSession(
            GameState gameState,
            GameType gameType)
        {
            GameState = gameState;
            GameType = gameType;
        }
    }
}
