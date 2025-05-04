using System;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattleActionsFacade : IBattleActionsFacade
    {
        private readonly IBattleTurnsService _battleTurnsService;
        private readonly IMovementsService _movementsService;
        private readonly IAttacksService _attacksService;

        private readonly Subject<Guid> _movementCompleted = new();
        private readonly Subject<AttackEvent> _attackDone = new();

        public Observable<Guid> OnMovementCompleted => _movementCompleted.AsObservable();
        public Observable<AttackEvent> OnAttackDone => _attackDone.AsObservable();

        public BattleActionsFacade(
            IBattleTurnsService battleTurnsService,
            IMovementsService movementsService,
            IAttacksService attacksService)
        {
            _battleTurnsService = battleTurnsService;
            _movementsService = movementsService;
            _attacksService = attacksService;
        }

        public async UniTask MoveAsync(Tile targetTile)
        {
            var currentActiveGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            if (currentActiveGameObject is Unit unit)
            {
                await _movementsService.MoveAsync(targetTile, unit);
                _movementCompleted.OnNext(unit.Id);
            }

            return;
        }

        public async UniTask MeleeAttackAsync(Unit defender, Tile targetTile)
        {
            var currentActiveGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            if (currentActiveGameObject is Unit attacker)
            {
                await _attacksService.MeleeAttackAsync(attacker, defender, targetTile);
                var attackEvent = new AttackEvent(attacker, defender);
                _attackDone.OnNext(attackEvent);
            }

            return;
        }
    }
}
