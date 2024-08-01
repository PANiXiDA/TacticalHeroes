using Assets.Scripts.Enumerations;
using Unity.VisualScripting;

namespace Assets.Scripts.Units.Neutral
{
    public class BaseNeutralUnit : BaseUnit
    {
        protected override void Awake() 
        {
            base.Awake();
            Faction = Factions.Neutral;
        }
    }
}
