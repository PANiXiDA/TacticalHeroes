using Assets.Scripts.GameEngine.DTO.DamageCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using System;

namespace Assets.Scripts.GameEngine.Implementations
{
    public class DamageCalculator : IDamageCalculator
    {
        private const int MinimalStatsModifier = 1;
        private const double StatsModifierCoefficient = 0.05;
        private const double Epsilon = 1e-9;

        private static readonly Random _random = new Random();

        public int CalculateDamage(DamageContext context)
        {
            double baseRandomDamage = context.AttackerMinDamage
                + _random.NextDouble() * (context.AttackerMaxDamage - context.AttackerMinDamage)
                + Epsilon;

            int delta = context.AttackerAttack - context.DefenderDefence;
            double multiplier = Math.Pow(MinimalStatsModifier + StatsModifierCoefficient * Math.Abs(delta), Math.Sign(delta));

            int damageOneObject = Convert.ToInt32(baseRandomDamage * multiplier);
            int damageManyObjects = context.AttackerCount * damageOneObject;

            int finalDamage = Convert.ToInt32(damageManyObjects * context.DamageModifier);

            return finalDamage;
        }

        public DamageResult ComputeCasualties(DefenderTakeDamageContext ctx)
        {
            int totalHpBeforeAttack = ctx.DefenderCurrentHealth + (ctx.DefenderCount - 1) * ctx.DefenderFullHealth;
            int totalHpAfterAttack = Math.Max(totalHpBeforeAttack - ctx.AttackerDamage, 0);

            int survivingUnits = (totalHpAfterAttack + ctx.DefenderFullHealth - 1) / ctx.DefenderFullHealth;
            int deadUnits = ctx.DefenderCount - survivingUnits;

            int remainingHealth = survivingUnits > 0
                ? totalHpAfterAttack - (survivingUnits - 1) * ctx.DefenderFullHealth
                : 0;

            return new DamageResult(
                deadUnits: deadUnits,
                survivingUnits: survivingUnits,
                remainingHealth: remainingHealth
            );
        }

    }
}
