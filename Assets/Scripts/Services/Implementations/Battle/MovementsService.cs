using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class MovementsService : IMovementsService
    {
        private List<Tile> _grid;
        private Unit _currentActiveUnit;

        private readonly IPathFinderCalculator _pathFinderCalculator;

        private readonly ReplaySubject<IReadOnlyList<Tile>> _reachableTilesReceived = new(1);
        private readonly Subject<MovementPath> _pathComputed = new();
        private readonly Subject<Guid> _movementCompleted = new();

        private readonly Dictionary<Guid, UniTaskCompletionSource> _moveSources = new();

        public Observable<IReadOnlyList<Tile>> OnReachableTilesReceived => _reachableTilesReceived.AsObservable();
        public Observable<MovementPath> OnPathComputed => _pathComputed.AsObservable();
        public Observable<Guid> OnMovementCompleted => _movementCompleted.AsObservable();

        public MovementsService(IPathFinderCalculator pathFinderCalculator)
        {
            _pathFinderCalculator = pathFinderCalculator;
        }

        public UniTask GetReachableTilesAsync(List<Tile> grid, Unit unit)
        {
            var startTile = grid.First(tile => tile.OccupiedUnitId == unit.Id);

            var context = new PathfindingContext(
                moveRange: unit.Speed,
                ignoringObstacles: unit.Abilities.Any(ability => ability.Type == AbilityType.Fly),
                grid: grid,
                start: startTile);

            var reachableTiles = _pathFinderCalculator.GetReachableTiles(context).Where(tile => tile != startTile).ToList();
            _reachableTilesReceived.OnNext(reachableTiles);

            SetCache(grid, unit);

            return UniTask.CompletedTask;
        }

        public UniTask MoveAsync(Tile targetTile)
        {
            if (_moveSources.ContainsKey(_currentActiveUnit.Id))
            {
                return _moveSources[_currentActiveUnit.Id].Task;
            }
            _reachableTilesReceived.OnNext(Array.Empty<Tile>());

            var startTile = _grid.First(tile => tile.OccupiedUnitId == _currentActiveUnit.Id);

            var context = new PathfindingContext(
                moveRange: _currentActiveUnit.Speed,
                ignoringObstacles: _currentActiveUnit.Abilities.Any(ability => ability.Type == AbilityType.Fly),
                grid: _grid,
                start: startTile,
                target: targetTile);

            var path = _pathFinderCalculator.GetPath(context);

            UpdateOccupiedTile(startTile, targetTile);

            var movementPath = new MovementPath(_currentActiveUnit.Id, path);
            _pathComputed.OnNext(movementPath);

            var task = new UniTaskCompletionSource();
            _moveSources[_currentActiveUnit.Id] = task;

            return task.Task;
        }

        public void NotifyMovementCompleted(Guid unitId)
        {
            if (_moveSources.TryGetValue(unitId, out var task))
            {
                task.TrySetResult();
                _moveSources.Remove(unitId);
            }

            _movementCompleted.OnNext(unitId);
        }

        private void SetCache(List<Tile> grid, Unit unit)
        {
            _grid = grid;
            _currentActiveUnit = unit;
        }

        private void UpdateOccupiedTile(Tile oldTile, Tile newTile)
        {
            oldTile.OccupiedUnitId = null;
            oldTile.IsWalkable = true;

            newTile.OccupiedUnitId = _currentActiveUnit.Id;
            newTile.IsWalkable = false;
        }
    }
}
