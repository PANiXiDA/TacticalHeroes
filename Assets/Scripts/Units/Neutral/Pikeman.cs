using Assets.Scripts.IActions;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Neutral
{
    public class Pikeman : BaseNeutralUnit
    {
        public override async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            if (UnitResponse)
            {
                await _meleeAttack.MeleeAttack(defender, attacker);
            }
            UnitResponse = false;
            await _takeDamage.TakeMeleeDamage(attacker, defender, damageCalculator);
        }
    }
}
