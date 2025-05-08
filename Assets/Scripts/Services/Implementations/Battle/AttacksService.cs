using Assets.Scripts.GameEngine.DTO.DamageCalculator;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using Assets.Scripts.Domain.GameEngine.DTO.Extensions;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;
using Assets.Scripts.GameEngine.Domain.Core;
using System;
using Assets.Scripts.GameEngine.Domain.Enums;
using System.Linq;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class AttacksService : IAttacksService
    {
        private readonly IDamageCalculator _damageCalculator;
        private readonly IMovementsService _movementsService;
        private readonly IGridsService _gridsService;

        public AttacksService(
            IDamageCalculator damageCalculator,
            IMovementsService movementsService,
            IGridsService gridsService)
        {
            _damageCalculator = damageCalculator;
            _movementsService = movementsService;
            _gridsService = gridsService;
        }

        public async UniTask MeleeAttackAsync(Unit attacker, Unit defender, Tile targetTile)
        {
            var damageModifier = 1;
            var count = attacker.Count;

            await _movementsService.MoveAsync(targetTile, attacker);

            var damageContext = new DamageContext(
                attackerAttack: attacker.EffectiveAttack(),
                attackerMinDamage: attacker.EffectiveMinDamage(),
                attackerMaxDamage: attacker.EffectiveMaxDamage(),
                attackerCount: count,
                defenderDefence: defender.EffectiveDefence(),
                damageModifier: damageModifier);
            var damage = _damageCalculator.CalculateDamage(damageContext);

            var defenderTakeDamageContext = new DefenderTakeDamageContext(
                defenderCurrentHealth: defender.CurrentHealth,
                defenderFullHealth: defender.EffectiveHealth(),
                defenderCount: defender.Count,
                attackerDamage: damage);
            var damageResult = _damageCalculator.ComputeCasualties(defenderTakeDamageContext);

            UpdateDefenderStats(defender, damageResult);

            return;
        }

        public UniTask RangeAttackAsync(GameObject attacker, Unit defender)
        {
            var damageModifier = 1d;
            var count = 1;
            var attackerAttack = attacker.Attack;
            var attackerMinDamage = attacker.MinDamage;
            var attackerMaxDamage = attacker.MaxDamage;

            if (attacker is Unit unit)
            {
                if (!unit.Range.HasValue || !unit.Arrows.HasValue || !unit.Abilities.Any(ability => ability.Type == AbilityType.Archer))
                {
                    return UniTask.CompletedTask;
                }

                count = unit.Count;
                attackerAttack = unit.EffectiveAttack();
                attackerMinDamage = unit.EffectiveMinDamage();
                attackerMaxDamage = unit.EffectiveMaxDamage();
                damageModifier *= GetDistanceModifier(unit.Id, defender.Id, unit.Range.Value);

                UpdateAttackerArrows(unit);
            }

            var damageContext = new DamageContext(
                attackerAttack: attackerAttack,
                attackerMinDamage: attackerMinDamage,
                attackerMaxDamage: attackerMaxDamage,
                attackerCount: count,
                defenderDefence: defender.EffectiveDefence(),
                damageModifier: damageModifier);
            var damage = _damageCalculator.CalculateDamage(damageContext);

            var defenderTakeDamageContext = new DefenderTakeDamageContext(
                defenderCurrentHealth: defender.CurrentHealth,
                defenderFullHealth: defender.EffectiveHealth(),
                defenderCount: defender.Count,
                attackerDamage: damage);
            var damageResult = _damageCalculator.ComputeCasualties(defenderTakeDamageContext);

            UpdateDefenderStats(defender, damageResult);

            return UniTask.CompletedTask;
        }

        private double GetDistanceModifier(Guid attackerId, Guid defenderId, int range)
        {
            var attackerTile = _gridsService.GetTile(attackerId);
            var defenderTile = _gridsService.GetTile(defenderId);

            var distance = _gridsService.GetDistance(attackerTile, defenderTile);

            return range > distance ? 1d : 0.5d;
        }

        private void UpdateDefenderStats(Unit defender, DamageResult damageResult)
        {
            defender.CurrentHealth = damageResult.RemainingHealth;
            defender.Count = damageResult.SurvivingUnits;
        }

        private void UpdateAttackerArrows(Unit attacker) => attacker.Arrows--;
    }
}
