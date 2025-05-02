using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using R3;

using Cysharp.Threading.Tasks;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.Domain.DTO.Models;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattleTurnsService : IBattleTurnsService
    {
        private readonly IATBCalculator _atbCalculator;

        private readonly ReplaySubject<GameObject> _turnStarted = new(1);
        private readonly Subject<GameObject> _turnEnded = new();

        private GameObject _currentActiveGameObject;
        private List<GameEntity> _nextAtb = new();

        public Observable<GameObject> OnTurnStarted => _turnStarted.AsObservable();
        public Observable<GameObject> OnTurnEnded => _turnEnded.AsObservable();

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

            SetCache(gameObjects.FirstOrDefault(item => item.Id == result.NextGameObjectId), result.UpdatedATBState);

            _turnStarted.OnNext(_currentActiveGameObject);

            return UniTask.CompletedTask;
        }

        public UniTask CompleteTurnAsync(RoundState roundState, List<RoundState> gameHistory)
        {
            RemoveDeadUnits(roundState.Units);
            ClearGridFromDeadUnits(roundState.Grid, roundState.Units);
            gameHistory.Add(roundState);
            roundState.ATB = _nextAtb;

            _turnEnded.OnNext(_currentActiveGameObject);

            return UniTask.CompletedTask;
        }

        private void SetCache(GameObject gameObject, List<GameEntity> atb)
        {
            _currentActiveGameObject = gameObject;
            _nextAtb = atb;
        }

        private void RemoveDeadUnits(List<Unit> units)
        {
            _nextAtb.RemoveAll(atbItem => units.Any(unit => unit.Id == atbItem.GameEntityId && unit.Count <= 0));
            units.RemoveAll(unit => unit.Count <= 0);
        }

        private void ClearGridFromDeadUnits(List<Tile> grid, List<Unit> units)
        {
            foreach (var tile in grid)
            {
                if (tile.OccupiedUnitId.HasValue && !units.Any(unit => unit.Id == tile.OccupiedUnitId.Value))
                {
                    tile.OccupiedUnitId = null;
                    tile.IsWalkable = true;
                }
            }
        }
    }
}
