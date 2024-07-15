using Assets.Scripts.Interfaces;
using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine.Analytics;

namespace Assets.Scripts.Actions.TakeDamage
{
    public class DefaultTakeDamage : ITakeDamage
    {
        public async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender)
        {
            int damage = CalculateDamage(attacker, defender);

            defender.animator.Play("TakeDamage");

            if (defender.UnitCurrentHealth > damage)
            {
                defender.UnitCurrentHealth -= damage;
            }
            else
            {
                int countDeathUnits = (damage - defender.UnitCurrentHealth) / defender.UnitFullHealth + 1;
                defender.UnitCount = defender.UnitCount - countDeathUnits > 0 ? defender.UnitCount - countDeathUnits : 0;
                defender.UnitCurrentHealth = defender.UnitFullHealth - ((damage - defender.UnitCurrentHealth) - defender.UnitFullHealth * (countDeathUnits - 1));
                UnitFactory.Instance.CreateOrUpdateUnitVisuals(defender);
                MenuManager.Instance.UpdatePortraitsInfo(defender);
            }

            if (defender.UnitCount > 0)
            {
                await UniTask.Delay(1000);
            }
        }
        public async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender)
        {
            int damage = CalculateDamage(attacker, defender);

            defender.animator.Play("TakeDamage");

            if (defender.UnitCurrentHealth > damage)
            {
                defender.UnitCurrentHealth -= damage;
            }
            else
            {
                int countDeathUnits = (damage - defender.UnitCurrentHealth) / defender.UnitFullHealth + 1;
                defender.UnitCount = defender.UnitCount - countDeathUnits > 0 ? defender.UnitCount - countDeathUnits : 0;
                defender.UnitCurrentHealth = defender.UnitFullHealth - ((damage - defender.UnitCurrentHealth) - defender.UnitFullHealth * (countDeathUnits -1));
                UnitFactory.Instance.CreateOrUpdateUnitVisuals(defender);
                MenuManager.Instance.UpdatePortraitsInfo(defender);
            }

            if (defender.UnitCount > 0)
            {
                await UniTask.Delay(1000);
            }
        }

        public int CalculateDamage(BaseUnit attacker, BaseUnit defender)
        {
            double baseDamage = UnityEngine.Random.Range(attacker.UnitMinDamage, attacker.UnitMaxDamage);
            double damageModifier = attacker.UnitAttack > defender.UnitDefence ?
                (1 + 0.05 * (attacker.UnitAttack - defender.UnitDefence)) :
                (1 / (1 + 0.05 * (defender.UnitDefence - attacker.UnitAttack)));

            int damage = (int)(baseDamage * damageModifier * attacker.UnitCount);

            var distance = math.ceil(math.sqrt(math.pow(GridManager.Instance.GetTileCoordinate(defender.OccupiedTile).x - attacker.OccupiedTile.Position.x, 2) +
                math.pow(GridManager.Instance.GetTileCoordinate(defender.OccupiedTile).y - attacker.OccupiedTile.Position.y, 2)));

            if (attacker.UnitRange < distance)
            {
                damage /= 2;
            }

            MenuManager.Instance.ShowDamage(attacker, defender, damage);

            return damage;
        }
    }
}
