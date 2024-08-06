using Assets.Scripts.IActions;
using Assets.Scripts.UI;

namespace Assets.Scripts.Actions.Damage
{
    public class PercentDefaultDamage : DefaultDamage, IDamage
    {
        public double _percentDamage {  get; set; }

        public PercentDefaultDamage(double percentDamage)
        {
            _percentDamage = percentDamage;
        }

        public override int CalculateDamage(BaseUnit attacker, BaseUnit defender)
        {
            return (int)(base.CalculateDamage(attacker, defender) * _percentDamage);
        }

    }
}
