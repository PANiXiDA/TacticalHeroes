using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.DamageCalculator;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class AttacksService : IAttacksService
    {
        private readonly IDamageCalculator _damageCalculator;
        private readonly IMovementsService _movementsService;

        private readonly Subject<AttackEvent> _attackDone = new();

        public Observable<AttackEvent> OnAttackDone => _attackDone.AsObservable();

        public AttacksService(
            IDamageCalculator damageCalculator,
            IMovementsService movementsService)
        {
            _damageCalculator = damageCalculator;
            _movementsService = movementsService;
        }

        public async UniTask MeleeAttackAsync(GameObject attacker, Unit defender, Tile targetTile)
        {
            var damageModifier = 1;
            var count = 1;

            if (attacker is Unit unit && targetTile.IsWalkable && !targetTile.OccupiedUnitId.HasValue)
            {
                await _movementsService.MoveAsync(targetTile);
                count = unit.Count;
            }

            var damageContext = new DamageContext(
                attackerAttack: attacker.Attack,
                attackerMinDamage: attacker.MinDamage,
                attackerMaxDamage: attacker.MaxDamage,
                attackerCount: count,
                defenderDefense: defender.Defence,
                damageModifier: damageModifier);
            var damage = _damageCalculator.CalculateDamage(damageContext);

            var defenderTakeDamageContext = new DefenderTakeDamageContext(
                defenderCurrentHealth: defender.CurrentHealth,
                defenderFullHealth: defender.FullHealth,
                defenderCount: defender.Count,
                attackerDamage: damage);
            var damageResult = _damageCalculator.ComputeCasualties(defenderTakeDamageContext);

            UpdateDefenderStats(defender, damageResult);

            var attackEvent = new AttackEvent(attacker, defender);
            _attackDone.OnNext(attackEvent);

            return;
        }

        private void UpdateDefenderStats(Unit defender, DamageResult damageResult)
        {
            defender.CurrentHealth = damageResult.RemainingHealth;
            defender.Count = damageResult.SurvivingUnits;
        }
    }
}
