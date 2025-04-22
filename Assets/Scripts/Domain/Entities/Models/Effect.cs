using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.Domain.Entities.Models
{
    public class Effect
    {
        public int Id { get; set; }
        public EffectType Type { get; set; }
        public double Value { get; set; }
        public double Duration { get; set; }
        public string Parameters { get; set; }

        public Effect(
            int id,
            EffectType type,
            double value,
            double duration,
            string parameters)
        {
            Id = id;
            Type = type;
            Value = value;
            Duration = duration;
            Parameters = parameters;
        }
    }
}
