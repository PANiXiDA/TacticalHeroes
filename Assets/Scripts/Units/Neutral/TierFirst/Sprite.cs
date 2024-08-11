using Assets.Scripts.Actions.Attack.MeleeAttack;

namespace Assets.Scripts.Units.Neutral
{
    public class Sprite : BaseNeutralUnit
    {
        protected override void Start()
        {
            base.Start();
            _meleeAttack = new NoResponseAttack(_damageCalculator);
        }
    }
}
