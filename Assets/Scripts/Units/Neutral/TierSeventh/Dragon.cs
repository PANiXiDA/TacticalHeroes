using Assets.Scripts.Actions.Attack.MeleeAttack;
using Assets.Scripts.Actions.Damage;
using Assets.Scripts.Actions.Move;
using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Units.Neutral;
using Cysharp.Threading.Tasks;

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
