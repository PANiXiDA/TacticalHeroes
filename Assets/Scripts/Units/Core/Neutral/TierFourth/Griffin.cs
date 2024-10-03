using Assets.Scripts.Units.IActions;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Neutral
{
    public class Griffin : BaseNeutralUnit
    {
        public override async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            if (!UnitResponse)
            {
                UnitResponse = true;
            }
            await base.TakeMeleeDamage(attacker, defender, damageCalculator);
        }
    }
}
