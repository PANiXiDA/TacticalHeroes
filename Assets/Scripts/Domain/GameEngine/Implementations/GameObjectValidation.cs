using System.Linq;

using Assets.Scripts.Domain.GameEngine.Interfaces;
using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.Domain.GameEngine.Implementations
{
    public class GameObjectValidation : IGameObjectValidation
    {
        public bool IsValidArcher(GameObject attacker)
        {
            return attacker is Hero hero
                || (attacker is Unit unit 
                && unit.Range.HasValue 
                && unit.Arrows.HasValue
                && unit.Arrows.Value > 0
                && unit.Abilities.Any(ability => ability.Type == AbilityType.Archer));
        }
    }
}
