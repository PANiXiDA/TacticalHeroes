using Assets.Scripts.Units.Actions.Attack.MeleeAttack;
namespace Assets.Scripts.Units.Neutral
{
    public class MinotaurKing : BaseNeutralUnit
    {
        protected override void Start()
        {
            base.Start();
            _meleeAttack = new LineMeleeAttack(_damageCalculator);
        }
    }
}
