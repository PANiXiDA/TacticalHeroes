using Assets.Scripts.Units.IActions;

namespace Assets.Scripts.Units.Actions.Damage
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
