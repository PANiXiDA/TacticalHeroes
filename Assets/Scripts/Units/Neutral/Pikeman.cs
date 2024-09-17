using Assets.Scripts.IActions;
using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Neutral
{
    public class Pikeman : BaseNeutralUnit
    {
        public override async UniTask TakeMeleeDamage(BaseUnit attacker, BaseUnit defender, IDamage damageCalculator)
        {
            if (UnitResponse && defender.Side != GameManager.Instance.CurrentSide)
            {
                await _meleeAttack.MeleeAttack(defender, attacker);
            }
            UnitResponse = false;
            if (attacker.UnitCount > 0)
            {
                await _takeDamage.TakeMeleeDamage(attacker, defender, damageCalculator);
            }
            else
            {
                TurnManager.Instance.EndTurn(attacker, true);
            }
        }
    }
}
