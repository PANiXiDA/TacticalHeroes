using System.Collections.Generic;

using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IMovementsService
    {
        Observable<IReadOnlyList<Tile>> OnReachableTilesReceived { get; }
        Observable<IReadOnlyList<Tile>> OnPathComputed { get; }
        UniTask GetReachableTilesAsync(List<Tile> grid, Unit unit);
        UniTask GetPathAsync(Tile targetTile);
    }
}
