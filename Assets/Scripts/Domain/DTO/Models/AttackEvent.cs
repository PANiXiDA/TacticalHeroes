using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.Domain.DTO.Models
{
    public class AttackEvent
    {
        public GameObject Attacker { get; }
        public Unit Defender { get; }
        public int Damage { get; set; }
        public int Deaths { get; set; }
        public bool IsRangeAttack { get; }

        public AttackEvent(
            GameObject attacker,
            Unit defender,
            int damage,
            int deaths,
            bool isRangeAttack)
        {
            Attacker = attacker;
            Defender = defender;
            Damage = damage;
            Deaths = deaths;
            IsRangeAttack = isRangeAttack;
        }
    }
}
