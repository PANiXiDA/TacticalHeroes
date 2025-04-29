using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using R3;

using System;
using Cysharp.Threading.Tasks;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.Domain.DTO.Models;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattleTurnsService : IBattleTurnsService
    {
        private readonly IATBCalculator _atbCalculator;

        private readonly ReplaySubject<Guid> _turnStarted = new(1);

        private List<GameEntity> _nextAtb = new();

        public Observable<Guid> OnTurnStarted => _turnStarted.AsObservable();

        public BattleTurnsService(IATBCalculator atbCalculator)
        {
            _atbCalculator = atbCalculator;
        }

        public UniTask StartNextTurnAsync(List<GameObject> gameObjects, List<GameEntity> atb)
        {
            var initiatives = gameObjects
                .Select(gameObject => new GameEntityInitiative(gameObject.Id, gameObject.Initiative))
                .ToList();

            var result = _atbCalculator.GetNextTurn(new ATBCalculationContext()
            {
                GameEntitiesInitiatives = initiatives,
                CurrentATBState = atb
            });

            _turnStarted.OnNext(result.NextGameObjectId);
            _nextAtb = result.UpdatedATBState;

            return UniTask.CompletedTask;
        }

        public UniTask CompleteTurnAsync(RoundState roundState, List<RoundState> gameHistory)
        {
            gameHistory.Add(roundState);
            roundState.ATB = _nextAtb;

            return UniTask.CompletedTask;
        }
    }
}
