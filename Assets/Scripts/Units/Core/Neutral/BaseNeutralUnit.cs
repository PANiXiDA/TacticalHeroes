namespace Assets.Scripts.Units.Neutral
{
    public class BaseNeutralUnit : BaseUnit
    {
        protected override void Awake() 
        {
            base.Awake();
            Faction = Faction.Neutral;
            UnitMorale = 5;
            UnitLuck = 5;
        }
    }
}
