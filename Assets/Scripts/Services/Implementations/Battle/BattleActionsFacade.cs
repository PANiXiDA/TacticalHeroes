using System;

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
        private readonly IATBService _atbService;

        private readonly Subject<Guid> _movementCompleted = new();
        private readonly Subject<Guid> _attackDone = new();
        private readonly Subject<Guid> _defenceDone = new();
        private readonly Subject<Guid> _waitDone = new();

        public Observable<Guid> OnMovementCompleted => _movementCompleted.AsObservable();
        public Observable<Guid> OnAttackDone => _attackDone.AsObservable();
        public Observable<Guid> OnDefenceDone => _defenceDone.AsObservable();
        public Observable<Guid> OnWaitDone => _waitDone.AsObservable();

        public BattleActionsFacade(
            IBattleTurnsService battleTurnsService,
            IMovementsService movementsService,
            IAttacksService attacksService,
            IBuffsDebuffsService buffsDebuffsService,
            IATBService atbService)
        {
            _battleTurnsService = battleTurnsService;
            _movementsService = movementsService;
            _attacksService = attacksService;
            _buffsDebuffsService = buffsDebuffsService;
            _atbService = atbService;
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
                _attackDone.OnNext(currentActiveGameObject.Id);
            }

            return;
        }

        public async UniTask RangeAttackAsync(Unit defender)
        {
            var currentActiveGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            await _attacksService.RangeAttackAsync(currentActiveGameObject, defender);
            _attackDone.OnNext(currentActiveGameObject.Id);

            return;
        }

        public UniTask DefenceAsync()
        {
            var currentActiveGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            if (currentActiveGameObject is Unit unit)
            {
                var position = _atbService.GetGameObjectAtbPosition(unit.Id);
                var duration = (EndPosition - position) / unit.EffectiveInitiative();
                _buffsDebuffsService.AddDefenceEffect(unit, duration);
            }
            _defenceDone.OnNext(currentActiveGameObject.Id);

            return UniTask.CompletedTask;
        }

        public UniTask WaitAsync()
        {
            var currentActiveGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            _atbService.UpdateAtb(
                activeGameObjectId: currentActiveGameObject.Id,
                isWait: true);
            _waitDone.OnNext(currentActiveGameObject.Id);

            return UniTask.CompletedTask;
        }
    }
}
