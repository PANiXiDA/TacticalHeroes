using Assets.Scripts.Units.Actions.Damage;
using Assets.Scripts.Units.IActions;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Actions.TakeDamage
{
    public class BigShieldTakeDamage : DefaultTakeDamage, ITakeDamage
    {
        public override async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            IDamage halfDamageCalculator = new PercentDefaultDamage(0.5);
            await base.TakeRangeDamage(attacker, defender, halfDamageCalculator);
        }
    }
}
