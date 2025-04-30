using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle.States;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Implementations.Battle.States.Core
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
