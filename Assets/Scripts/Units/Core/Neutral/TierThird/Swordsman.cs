using Assets.Scripts.Units.Actions.TakeDamage;

namespace Assets.Scripts.Units.Neutral
{
    public class Swordsman : BaseNeutralUnit
    {
        protected override void Start()
        {
            base.Start();
            _takeDamage = new BigShieldTakeDamage();        
        }
    }
}
