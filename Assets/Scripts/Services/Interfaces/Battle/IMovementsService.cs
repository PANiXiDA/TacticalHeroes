using System;
using System.Collections.Generic;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IMovementsService
    {
        Observable<IReadOnlyList<Tile>> OnReachableTilesReceived { get; }
        Observable<MovementPath> OnPathComputed { get; }
        void ClearReachableTiles();
        UniTask GetReachableTilesAsync(Unit unit);
        UniTask MoveAsync(Tile targetTile, Unit unit);
        void NotifyMovementCompleted(Guid unitId);
    }
}
