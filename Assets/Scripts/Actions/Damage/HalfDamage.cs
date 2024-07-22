using Assets.Scripts.IActions;
using Assets.Scripts.UI;

namespace Assets.Scripts.Actions.Damage
{
    public class HalfDamage : IDamage
    {
        private IDamage _damage;
        public HalfDamage(IDamage damage)
        {
            _damage = damage;
        }

        public (int damage, int deathCount) CalculateDamageAndDeathUnit(BaseUnit attacker, BaseUnit defender)
        {
            int damage = CalculateDamage(attacker, defender);
            int deathCount = CalculateDeathCount(defender, damage);

            MenuManager.Instance.DisplayDamageWithDeathCountInChat(attacker, defender, damage, deathCount);
            UnitFactory.Instance.CreateDamageVisuals(attacker, defender, damage, deathCount);

            return (damage, deathCount);
        }
        public int CalculateDamage(BaseUnit attacker, BaseUnit defender)
        {
            return _damage.CalculateDamage(attacker, defender) / 2;
        }
        public int CalculateDeathCount(BaseUnit defender, int damage)
        {
            return _damage.CalculateDeathCount(defender, damage);
        }
    }
}
