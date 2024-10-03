using Assets.Scripts.Units.Actions.Attack.MeleeAttack;
using Assets.Scripts.Units.Actions.Attack.RangeAttack;
using Assets.Scripts.Units.Actions.Damage;
using Assets.Scripts.Units.IActions;
using Assets.Scripts.Units.Neutral;

namespace Assets.Scripts.Units.Heroes
{
    public class Lich : BaseNeutralUnit
    {
        private IDamage _damageMeleeCalculator;
        private IDamage _damageRangeCalculator;

        protected override void Start()
        {
            base.Start();

            _damageRangeCalculator = new DefaultDamage();
            _damageMeleeCalculator = new PercentDefaultDamage(0.5);

            _meleeAttack = new DefaultMeleeAttack(_damageMeleeCalculator);
            _rangeAttack = gameObject.AddComponent<ArealRangeAttack>();
            (_rangeAttack as ArealRangeAttack).Initialize(_damageRangeCalculator);

            UnitMorale = 0;
            UnitRange = 7;
            UnitArrows = 10;
        }
    }
}
