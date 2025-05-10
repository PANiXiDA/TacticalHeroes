using System.Linq;

using Assets.Scripts.Domain.GameEngine.Interfaces;
using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.Domain.Enums;

namespace Assets.Scripts.Domain.GameEngine.Implementations
{
    public class AttackCalculator : IAttackCalculator
    {
        public int GetCountMeleeAttacks(Unit unit)
        {
            if (unit.Abilities.Any(ability => ability.Type == AbilityType.DoubleDamage))
            {
                return 2;
            }

            return 1;
        }

        public int GetCountRangeAttacks(GameObject gameObject)
        {
            if (gameObject is Unit unit)
            {
                if (unit.Abilities.Any(ability => ability.Type == AbilityType.DoubleRangeAttack))
                {
                    return 2;
                }
            }

            return 1;
        }

        public bool HasResponseMeleeAttack(Unit unit, bool isResponseAttack = false)
        {
            if (isResponseAttack)
            {
                return false;
            }
            if (unit.Count <= 0)
            {
                return false;
            }
            if (unit.Abilities.Any(ability => ability.Type == AbilityType.UnlimitResponce))
            {
                return true;
            }

            return unit.HasResponseMeleeAttack;
        }

        public bool HasResponseRangeAttack(GameObject gameObject, bool isResponseAttack = false)
        {
            if (isResponseAttack)
            {
                return false;
            }
            if (gameObject is not Unit unit)
            {
                return false;
            }
            if (unit.Count <= 0)
            {
                return false;
            }

            return unit.HasResponseRangeAttack;
        }
    }
}
