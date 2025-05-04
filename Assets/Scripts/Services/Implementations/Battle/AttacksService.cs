using Assets.Scripts.GameEngine.DTO.DamageCalculator;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class AttacksService : IAttacksService
    {
        private readonly IDamageCalculator _damageCalculator;
        private readonly IMovementsService _movementsService;

        public AttacksService(
            IDamageCalculator damageCalculator,
            IMovementsService movementsService)
        {
            _damageCalculator = damageCalculator;
            _movementsService = movementsService;
        }

        public async UniTask MeleeAttackAsync(Unit attacker, Unit defender, Tile targetTile)
        {
            var damageModifier = 1;
            var count = attacker.Count;

            await _movementsService.MoveAsync(targetTile, attacker);

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

            return;
        }

        private void UpdateDefenderStats(Unit defender, DamageResult damageResult)
        {
            defender.CurrentHealth = damageResult.RemainingHealth;
            defender.Count = damageResult.SurvivingUnits;
        }
    }
}
