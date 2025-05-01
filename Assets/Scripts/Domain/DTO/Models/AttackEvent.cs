using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.Domain.DTO.Models
{
    public class AttackEvent
    {
        public GameObject Attacker { get; }
        public Unit Defender { get; }

        public AttackEvent(
            GameObject attacker,
            Unit defender)
        {
            Attacker = attacker;
            Defender = defender;
        }
    }
}
