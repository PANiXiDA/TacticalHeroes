using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Damage;
using Assets.Scripts.IActions;
using Assets.Scripts.Units.Citadel;

namespace Assets.Scripts.Units.Enemies
{
    public class Cyclop : BaseCitadelUnit
    {
        private IDamage _damageMeleeCalculator;

        protected override void Start()
        {
            base.Start();
            _damageMeleeCalculator = new PercentDefaultDamage(0.5);
            _meleeAttack = new DefaultMeleeAttack(_damageMeleeCalculator);
        }
    }
}
