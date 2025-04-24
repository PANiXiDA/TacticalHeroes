using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Effect
    {
        public EffectType Type { get; set; }
        public double Value { get; set; }
        public double Duration { get; set; }
        public string Parameters { get; set; }

        public Effect(
            EffectType type,
            double value,
            double duration,
            string parameters)
        {
            Type = type;
            Value = value;
            Duration = duration;
            Parameters = parameters;
        }
    }
}
