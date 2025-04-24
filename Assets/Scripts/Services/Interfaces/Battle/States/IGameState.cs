using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Interfaces.Battle.States
{
    public interface IGameState
    {
        UniTask<GameState?> EnterAsync(GameSession gameSession);
        void Exit();
    }
}
