using System;
using System.Collections.Generic;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IAttacksService
    {
        Observable<List<AttackEvent>> OnAttackPrepared { get; }
        UniTask MeleeAttackAsync(Unit attacker, Unit defender, Tile targetTile, bool isResponseAttack = false);
        UniTask RangeAttackAsync(GameObject attacker, Unit defender, bool isResponseAttack = false);
        void NotifyAttackCompleted(Guid attackerId);
    }
}
