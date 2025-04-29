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
        UniTask GetReachableTilesAsync(List<Tile> grid, Unit unit);
    }
}
