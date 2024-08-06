using Assets.Scripts.Actions.TakeDamage;
using Assets.Scripts.IActions;
using Assets.Scripts.Interfaces;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Neutral
{
    public class Swordsman : BaseNeutralUnit
    {
        private ITakeDamage _takeDamage;
        protected override void Start()
        {
            base.Start();
            _takeDamage = new BigShieldTakeDamage();        
        }

        public override async UniTask TakeRangeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            await _takeDamage.TakeRangeDamage(attacker, defender, damageCalculator);
        }
    }
}
