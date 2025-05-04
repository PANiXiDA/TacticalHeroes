using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.GameEngine.Domain.Enums;

using DomainEffect = Assets.Scripts.GameEngine.Domain.Effect;

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

        public static DomainEffect MapToDomain(Effect entity)
        {
            return null; // TODO: переписать нормально, если текущая реализация эффекта сработает
        }

        public static List<DomainEffect> MapToDomains(List<Effect> entities)
        {
            return entities.Select(entity => MapToDomain(entity)).ToList();
        }
    }
}
