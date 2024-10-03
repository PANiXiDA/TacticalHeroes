using Assets.Scripts.Units.Actions.Damage;

namespace Assets.Scripts.Units.Citadel
{
    public class BaseCitadelUnit : BaseUnit
    {
        protected override void Awake()
        {
            base.Awake();
            Faction = Faction.Citadel;
            _damageCalculator = new PercentDefaultDamage(1.3);
        }
    }
}
