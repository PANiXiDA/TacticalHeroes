using System;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces;

namespace Assets.Scripts.Services.Implementations
{
    public class GameSessionsFactory : IGameSessionsFactory
    {
        private static readonly Guid[] AvailableBuildIds = new[]
        {
            Guid.Parse("2927a215-549b-496a-afd8-8163ec85970a"),
            Guid.Parse("5777e8cb-5bbf-4496-a06c-fe5ddcb89e96"),
        };

        private Random _random;

        public GameSessionsFactory()
        {
            _random = new Random();
        }

        public GameSession CreateDefault()
        {
            var gameSession = new GameSession(
                gameState: GameState.Default,
                gameType: GameType.Duel);

            var firstPlayer = new PlayerBattleData(
                id: 1,
                sessionId: null,
                buildId: GetRandomBuildId(),
                countMissedMoves: 0,
                side: PlayerSide.Left,
                teamNumber: 1,
                confirmedDeployment: false,
                columnsToDeployment: 0);
            gameSession.Players.Add(firstPlayer);

            var secondPlayer = new PlayerBattleData(
                id: 2,
                sessionId: null,
                buildId: GetRandomBuildId(),
                countMissedMoves: 0,
                side: PlayerSide.Right,
                teamNumber: 2,
                confirmedDeployment: false,
                columnsToDeployment: 0);
            gameSession.Players.Add(secondPlayer);

            return gameSession;
        }

        private Guid GetRandomBuildId()
        {
            int index = _random.Next(AvailableBuildIds.Length);
            return AvailableBuildIds[index];
        }
    }
}
