using System.Collections.Generic;
using System.Linq;

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

        private readonly ReplaySubject<IReadOnlyList<Tile>> _reachableTilesReceived = new(1);

        public Observable<IReadOnlyList<Tile>> OnReachableTilesReceived => _reachableTilesReceived.AsObservable();

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

            return UniTask.CompletedTask;
        }
    }
}
