using Assets.Scripts.UI;
using Assets.Scripts.Units.IActions;
using System;

namespace Assets.Scripts.Units.Actions.Damage
{
    public class DefaultDamage : IDamage
    {
        public bool isLuck { get; set; }
        public (int damage, int deathCount) CalculateDamageAndDeathUnit(BaseUnit attacker, BaseUnit defender)
        {
            int damage = CalculateDamage(attacker, defender);
            int deathCount = CalculateDeathCount(defender, damage);

            MenuManager.Instance.DisplayDamageWithDeathCountInChat(attacker, defender, damage, deathCount);
            UnitFactory.Instance.CreateDamageVisuals(attacker, defender, damage, deathCount);

            return (damage, deathCount);
        }

        public virtual int CalculateDamage(BaseUnit attacker, BaseUnit defender)
        {
            double baseDamage = UnityEngine.Random.Range(attacker.UnitMinDamage, attacker.UnitMaxDamage);
            double damageModifier = attacker.UnitAttack > (defender.UnitDefence + defender.UnitAdditionalDefence)
                ? 1 + 0.05 * (attacker.UnitAttack - (defender.UnitDefence + defender.UnitAdditionalDefence))
                : 1 / (1 + 0.05 * ((defender.UnitDefence + defender.UnitAdditionalDefence) - attacker.UnitAttack));

            int damage = (int)(baseDamage * damageModifier * attacker.UnitCount);

            double distance = Math.Ceiling(Math.Sqrt(
                Math.Pow(GridManager.Instance.GetTileCoordinate(defender.OccupiedTile).x - attacker.OccupiedTile.Position.x, 2) +
                Math.Pow(GridManager.Instance.GetTileCoordinate(defender.OccupiedTile).y - attacker.OccupiedTile.Position.y, 2)));

            if (attacker.UnitRange < distance)
            {
                damage /= 2;
            }
         
            if (isLuck)
            {
                damage *= 2;
            }

            return damage;
        }

        public int CalculateDeathCount(BaseUnit defender, int damage)
        {
            int remainingHealth = damage - defender.UnitCurrentHealth;
            int deathCount = Math.Max(0, remainingHealth / defender.UnitFullHealth);

            if (damage >= defender.UnitCurrentHealth)
            {
                deathCount++;
            }

            return Math.Min(deathCount, defender.UnitCount);
        }
    }
}
