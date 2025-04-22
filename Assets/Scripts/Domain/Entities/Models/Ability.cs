using Assets.Scripts.GameEngine.Domain.Enums;

using System.Collections.Generic;

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
    }
}
