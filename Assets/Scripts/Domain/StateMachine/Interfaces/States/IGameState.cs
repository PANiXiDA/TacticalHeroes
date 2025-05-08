using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Domain.StateMachine.Interfaces.States
{
    public interface IGameState
    {
        UniTask<GameState?> EnterAsync(GameSession gameSession);
        void Exit();
    }
}
