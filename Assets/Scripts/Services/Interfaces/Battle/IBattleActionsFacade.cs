using System;

using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IBattleActionsFacade
    {
        Observable<Guid> OnMovementCompleted { get; }
        Observable<Guid > OnAttackDone { get; }
        Observable<Guid> OnDefenceDone { get; }
        Observable<Guid> OnWaitDone { get; }
        Observable<Guid> OnSurrenderDone { get; }
        UniTask MoveAsync(Tile targetTile);
        UniTask MeleeAttackAsync(Unit defender, Tile targetTile);
        UniTask RangeAttackAsync(Unit defender);
        UniTask DefenceAsync();
        UniTask WaitAsync();
        UniTask SurrenderAsync();
    }
}
