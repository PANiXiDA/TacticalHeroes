using Assets.Scripts.GameEngine.Domain.Core;

namespace Assets.Scripts.Domain.GameEngine.Interfaces
{
    public interface IGameObjectValidation
    {
        bool IsValidArcher(GameObject attacker);
    }
}
