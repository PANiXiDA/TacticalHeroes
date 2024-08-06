using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Actions.TakeDamage
{
    public class DefaultTakeDamage : ITakeDamage
    {
        public virtual async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            (int damage, int countDeathUnits) = damageCalculator.CalculateDamageAndDeathUnit(attacker, defender);

            UnitManager.Instance.PlayAttackAnimation(attacker, defender);

            defender.animator.Play("TakeDamage");

            if (defender.UnitCurrentHealth > damage)
            {
                defender.UnitCurrentHealth -= damage;
            }
            else
            {
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
        public virtual async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            (int damage, int countDeathUnits) = damageCalculator.CalculateDamageAndDeathUnit(attacker, defender);

            defender.animator.Play("TakeDamage");

            if (defender.UnitCurrentHealth > damage)
            {
                defender.UnitCurrentHealth -= damage;
            }
            else
            {
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
    }
}
