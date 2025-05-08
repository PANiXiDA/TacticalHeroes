using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces.States;
using Assets.Scripts.Infrastructure.Models;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Domain.StateMachine.Implementations.States
{
    public sealed class BattleEndState : IGameState
    {
        public BattleEndState() { }

        public UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            return UniTask.FromResult<GameState?>(null);
        }

        public void Exit() { }
    }
}
