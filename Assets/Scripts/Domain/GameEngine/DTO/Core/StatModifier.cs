using Assets.Scripts.Domain.GameEngine.Domain.Enums;

namespace Assets.Scripts.Domain.GameEngine.Domain.Core
{
    public record StatModifier
    {
        public StatType Type { get; set; }
        public ModifierKind Kind { get; set; }
        public double Value { get; set; }

        public StatModifier(
            StatType type,
            ModifierKind kind,
            double value)
        {
            Type = type;
            Kind = kind;
            Value = value;
        }
    }
}
