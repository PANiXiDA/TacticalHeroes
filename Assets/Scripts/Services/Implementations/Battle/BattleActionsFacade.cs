using System;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Assets.Scripts.Domain.GameEngine.DTO.Extensions;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattleActionsFacade : IBattleActionsFacade
    {
        private const int EndPosition = 100;

        private readonly IBattleTurnsService _battleTurnsService;
        private readonly IMovementsService _movementsService;
        private readonly IAttacksService _attacksService;
        private readonly IBuffsDebuffsService _buffsDebuffsService;

        private readonly Subject<Guid> _movementCompleted = new();
        private readonly Subject<AttackEvent> _attackDone = new();
        private readonly Subject<Guid> _defenceDone = new();

        public Observable<Guid> OnMovementCompleted => _movementCompleted.AsObservable();
        public Observable<AttackEvent> OnAttackDone => _attackDone.AsObservable();
        public Observable<Guid> OnDefenceDone => _defenceDone.AsObservable();

        public BattleActionsFacade(
            IBattleTurnsService battleTurnsService,
            IMovementsService movementsService,
            IAttacksService attacksService,
            IBuffsDebuffsService buffsDebuffsService)
        {
            _battleTurnsService = battleTurnsService;
            _movementsService = movementsService;
            _attacksService = attacksService;
            _buffsDebuffsService = buffsDebuffsService;
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

        public UniTask DefenceAsync()
        {
            var currentActiveGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            if (currentActiveGameObject is Unit unit)
            {
                var position = _battleTurnsService.GetGameObjectAtbPosition(unit.Id);
                var duration = (EndPosition - position) / unit.EffectiveInitiative();
                _buffsDebuffsService.AddDefenceEffect(unit, duration);
            }
            _defenceDone.OnNext(currentActiveGameObject.Id);

            return UniTask.CompletedTask;
        }
    }
}
