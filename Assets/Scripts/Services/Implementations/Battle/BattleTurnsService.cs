using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Services.Interfaces.Battle;

using R3;

using Cysharp.Threading.Tasks;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.Domain.DTO.Models;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Common.Enumerations;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattleTurnsService : IBattleTurnsService
    {
        private readonly IATBService _atbService;
        private readonly IBuffsDebuffsService _buffsDebuffsService;

        private readonly ReplaySubject<GameObject> _turnStarted = new(1);
        private readonly ReplaySubject<GameObject> _turnEnded = new(1);

        private GameObject _currentActiveGameObject;

        public Observable<GameObject> OnTurnStarted => _turnStarted.AsObservable();
        public Observable<GameObject> OnTurnEnded => _turnEnded.AsObservable();

        public BattleTurnsService(
            IATBService atbService,
            IBuffsDebuffsService buffsDebuffsService)
        {
            _atbService = atbService;
            _buffsDebuffsService = buffsDebuffsService;
        }

        public GameObject GetCurrentActiveGameObject() => _currentActiveGameObject;

        public UniTask StartNextTurnAsync(List<GameObject> gameObjects, List<GameEntity> atb)
        {
            var result = _atbService.GetNextTurn(gameObjects, atb);

            _buffsDebuffsService.TickAllEffects(gameObjects.OfType<Unit>().ToList(), result.DeltaTime);
            SetCache(gameObjects.FirstOrDefault(item => item.Id == result.NextGameObjectId));

            UpdateCurrentActiveGameObjectStats(GetCurrentActiveGameObject());

            _turnStarted.OnNext(GetCurrentActiveGameObject());

            return UniTask.CompletedTask;
        }

        public UniTask CompleteTurnAsync(RoundState roundState, List<RoundState> gameHistory)
        {
            RemoveDeadUnits(roundState.Units);
            ClearGridFromDeadUnits(roundState.Grid, roundState.Units);
            gameHistory.Add(roundState);
            roundState.ATB = _atbService.GetAtb();

            _turnEnded.OnNext(GetCurrentActiveGameObject());

            if (roundState.LastCommand == CommandType.Wait)
            {
                _atbService.PredictNextTurns();
            }

            return UniTask.CompletedTask;
        }

        private void SetCache(GameObject gameObject)
        {
            _currentActiveGameObject = gameObject;
        }

        private void UpdateCurrentActiveGameObjectStats(GameObject gameObject)
        {
            if (gameObject is Unit unit)
            {
                unit.HasResponseMeleeAttack = true;
            }
        }

        private void RemoveDeadUnits(List<Unit> units)
        {
            _atbService.GetAtb().RemoveAll(atbItem => units.Any(unit => unit.Id == atbItem.GameEntityId && unit.Count <= 0));
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
