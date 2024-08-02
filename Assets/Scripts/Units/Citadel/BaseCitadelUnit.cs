using Assets.Scripts.Enumerations;

namespace Assets.Scripts.Units.Citadel
{
    public class BaseCitadelUnit : BaseUnit
    {
        protected override void Awake()
        {
            base.Awake();
            Faction = Faction.Citadel;
        }
    }
}
