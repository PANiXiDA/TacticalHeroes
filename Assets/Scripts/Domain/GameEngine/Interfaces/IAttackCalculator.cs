using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.Domain.GameEngine.Interfaces
{
    public interface IAttackCalculator
    {
        int GetCountMeleeAttacks(Unit unit);
        int GetCountRangeAttacks(GameObject gameObject);
        bool HasResponseMeleeAttack(Unit unit, bool isResponseAttack = false);
        bool HasResponseRangeAttack(GameObject gameObject, bool isResponseAttack = false);
    }
}
