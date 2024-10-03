using Assets.Scripts.Units.Actions.Attack.MeleeAttack;
using Assets.Scripts.Units.Citadel;

namespace Assets.Scripts.Units.Enemies
{
    public class Ogr : BaseCitadelUnit
    {
        protected override void Start()
        {
            base.Start();
            _meleeAttack = new DiscardMeleeAttack(_damageCalculator);
        }
    }
}
