using Assets.Scripts.GameEngine.Domain.Enums;

using System.Collections.Generic;
using System.Linq;

using DomainAbility = Assets.Scripts.GameEngine.Domain.Ability;

namespace Assets.Scripts.Domain.Entities.Models
{
    public class Ability
    {
        public int Id { get; set; }
        public AbilityType Type { get; set; }

        public List<Effect> Effects { get; set; } = new();

        public Ability(
            int id, 
            AbilityType type)
        {
            Id = id;
            Type = type;
        }

        public static DomainAbility MapToDomain(Ability entity)
        {
            return new DomainAbility(
                type: entity.Type,
                effects: Effect.MapToDomains(entity.Effects));
        }

        public static List<DomainAbility> MapToDomains(List<Ability> entities)
        {
            return entities.Select(entity => MapToDomain(entity)).ToList();
        }
    }
}
