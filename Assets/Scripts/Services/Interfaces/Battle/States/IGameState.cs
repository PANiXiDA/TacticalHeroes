using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Interfaces.Battle.States
{
    public interface IGameState
    {
        UniTask EnterAsync();
        void Exit();
    }
}
