using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Units.Neutral
{
    public class Elf : BaseNeutralUnit
    {
        protected override void Start()
        {
            base.Start();
            UnitRange = 100;
            UnitArrows = 16;
        }

        public override async UniTask RangeAttack(BaseUnit attacker, BaseUnit defender)
        {
            await _rangeAttack.RangeAttack(attacker, defender);
            if (defender.UnitCount > 0 && attacker.UnitCount > 0 && attacker.Side == GameManager.Instance.CurrentSide)
            {
                await _rangeAttack.RangeAttack(attacker, defender);
            }
            if (attacker.Side == GameManager.Instance.CurrentSide)
            {
                TurnManager.Instance.EndTurn(this);
            }
        }
    }
}
