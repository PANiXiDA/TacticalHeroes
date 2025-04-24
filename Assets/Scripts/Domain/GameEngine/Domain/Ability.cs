using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Ability
    {
        public AbilityType Type { get; set; }
        public List<Effect> Effects { get; set; }

        public Ability(
            AbilityType type,
            List<Effect> effects)
        {
            Type = type;
            Effects = effects;
        }
    }
}
