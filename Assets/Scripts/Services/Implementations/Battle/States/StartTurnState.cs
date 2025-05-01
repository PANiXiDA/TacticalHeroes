using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle.States
{
    public sealed class StartTurnState : IGameState
    {
        private readonly IBattleTurnsService _battleTurnsService;
        private readonly IMovementsService _movementsService;

        public StartTurnState(
            IBattleTurnsService battleTurnsService,
            IMovementsService movementsService)
        {
            _battleTurnsService = battleTurnsService;
            _movementsService = movementsService;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            var gameObjects = new List<GameObject>();
            gameObjects.AddRange(gameSession.RoundState.Units);
            gameObjects.AddRange(gameSession.RoundState.Heroes);

            _battleTurnsService.StartNextTurnAsync(gameObjects, gameSession.RoundState.ATB).Forget();

            var currentActiveGameObject = await _battleTurnsService.OnTurnStarted.FirstAsync();
            if (currentActiveGameObject is Unit unit)
            {
                _movementsService.GetReachableTilesAsync(gameSession.RoundState.Grid, unit).Forget();
            }

            return GameState.WaitAction;
        }

        public void Exit() { }
    }
}
