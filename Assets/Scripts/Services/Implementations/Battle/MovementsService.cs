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
        private readonly IPathFinderCalculator _pathFinderCalculator;
        private readonly IGridsService _gridsService;

        private readonly ReplaySubject<IReadOnlyList<Tile>> _reachableTilesReceived = new(1);
        private readonly Subject<MovementPath> _pathComputed = new();

        private readonly Dictionary<Guid, UniTaskCompletionSource> _moveSources = new();

        public Observable<IReadOnlyList<Tile>> OnReachableTilesReceived => _reachableTilesReceived.AsObservable();
        public Observable<MovementPath> OnPathComputed => _pathComputed.AsObservable();

        public MovementsService(
            IPathFinderCalculator pathFinderCalculator,
            IGridsService gridsService)
        {
            _pathFinderCalculator = pathFinderCalculator;
            _gridsService = gridsService;
        }

        public UniTask GetReachableTilesAsync(Unit unit)
        {
            var grid = _gridsService.GetGrid();
            var startTile = grid.First(tile => tile.OccupiedUnitId == unit.Id);

            var context = new PathfindingContext(
                moveRange: unit.Speed,
                ignoringObstacles: unit.Abilities.Any(ability => ability.Type == AbilityType.Fly),
                grid: grid,
                start: startTile);

            var reachableTiles = _pathFinderCalculator.GetReachableTiles(context).Where(tile => tile != startTile).ToList();
            _reachableTilesReceived.OnNext(reachableTiles);

            return UniTask.CompletedTask;
        }

        public UniTask MoveAsync(Tile targetTile, Unit unit)
        {
            if (_moveSources.ContainsKey(unit.Id))
            {
                return _moveSources[unit.Id].Task;
            }
            _reachableTilesReceived.OnNext(Array.Empty<Tile>());

            var grid = _gridsService.GetGrid();
            var startTile = grid.First(tile => tile.OccupiedUnitId == unit.Id);

            var context = new PathfindingContext(
                moveRange: unit.Speed,
                ignoringObstacles: unit.Abilities.Any(ability => ability.Type == AbilityType.Fly),
                grid: grid,
                start: startTile,
                target: targetTile);

            var path = _pathFinderCalculator.GetPath(context);

            UpdateOccupiedTile(startTile, targetTile, unit);

            var movementPath = new MovementPath(unit.Id, path);
            _pathComputed.OnNext(movementPath);

            var task = new UniTaskCompletionSource();
            _moveSources[unit.Id] = task;

            return task.Task;
        }

        public void NotifyMovementCompleted(Guid unitId)
        {
            if (_moveSources.TryGetValue(unitId, out var task))
            {
                task.TrySetResult();
                _moveSources.Remove(unitId);
            }
        }

        private void UpdateOccupiedTile(Tile oldTile, Tile newTile, Unit unit)
        {
            oldTile.OccupiedUnitId = null;
            oldTile.IsWalkable = true;

            newTile.OccupiedUnitId = unit.Id;
            newTile.IsWalkable = false;
        }
    }
}
