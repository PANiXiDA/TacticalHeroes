using System.Collections.Generic;

using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.GameEngine.Domain
{
    public class Ability
    {
        public AbilityType AbilityType { get; set; }
        public List<Effect> Effects { get; set; }

        public Ability(
            AbilityType abilityType,
            List<Effect> effects)
        {
            AbilityType = abilityType;
            Effects = effects;
        }
    }
}
