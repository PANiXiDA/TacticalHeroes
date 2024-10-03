using Assets.Scripts.Units.Actions.Attack.MeleeAttack;
using Assets.Scripts.Units.Actions.Damage;
using Assets.Scripts.Units.Actions.Move;
using Assets.Scripts.Units.Neutral;

namespace Assets.Scripts.Units.Heroes
{
    public class Dragon : BaseNeutralUnit
    {
        protected override void Start()
        {
            base.Start();
            _damageCalculator = new DefaultDamage();
            _move = new DefaultMove();
            _meleeAttack = new FieryBreathMeleeAttack(_damageCalculator);
        }
    }
}
