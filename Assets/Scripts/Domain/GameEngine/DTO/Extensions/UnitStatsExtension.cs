using Assets.Scripts.Domain.GameEngine.Domain.Enums;
using Assets.Scripts.GameEngine.Domain;

namespace Assets.Scripts.Domain.GameEngine.DTO.Extensions
{
    public static class UnitStatsExtension
    {
        public static int EffectiveAttack(this Unit unit) => unit.GetStatValue(StatType.Attack).Rounded;
        public static int EffectiveDefence(this Unit unit) => unit.GetStatValue(StatType.Defence).Rounded;
        public static int EffectiveHealth(this Unit unit) => unit.GetStatValue(StatType.Health).Rounded;
        public static int EffectiveMinDamage(this Unit unit) => unit.GetStatValue(StatType.MinDamage).Rounded;
        public static int EffectiveMaxDamage(this Unit unit) => unit.GetStatValue(StatType.MaxDamage).Rounded;
        public static double EffectiveInitiative(this Unit unit) => unit.GetStatValue(StatType.Initiative).Raw;
        public static int EffectiveSpeed(this Unit unit) => unit.GetStatValue(StatType.Speed).Rounded;
        public static int EffectiveRange(this Unit unit) => unit.GetStatValue(StatType.Range).Rounded;
        public static int EffectiveArrows(this Unit unit) => unit.GetStatValue(StatType.Arrows).Rounded;
        public static int EffectiveMorale(this Unit unit) => unit.GetStatValue(StatType.Morale).Rounded;
        public static int EffectiveLuck(this Unit unit) => unit.GetStatValue(StatType.Luck).Rounded;
    }
}
