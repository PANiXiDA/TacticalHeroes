using System;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces;

namespace Assets.Scripts.Services.Implementations
{
    public class GameSessionsFactory : IGameSessionsFactory
    {
        public GameSessionsFactory() { }

        public GameSession CreateDefault()
        {
            var gameSession = new GameSession(
                gameState: GameState.Default,
                gameType: GameType.Duel);

            var firstPlayer = new PlayerBattleData(
                id: 1,
                sessionId: null,
                buildId: Guid.Parse("f6bfec27-6fae-4e47-80f8-e8185f237d1a"),
                countMissedMoves: 0,
                side: PlayerSide.Left,
                teamNumber: 1,
                confirmedDeployment: false,
                columnsToDeployment: 0);
            gameSession.Players.Add(firstPlayer);

            var secondPlayer = new PlayerBattleData(
                id: 2,
                sessionId: null,
                buildId: Guid.Parse("89e84c39-f2b4-440a-9bf0-99a693960065"),
                countMissedMoves: 0,
                side: PlayerSide.Right,
                teamNumber: 2,
                confirmedDeployment: false,
                columnsToDeployment: 0);
            gameSession.Players.Add(secondPlayer);

            return gameSession;
        }
    }
}
