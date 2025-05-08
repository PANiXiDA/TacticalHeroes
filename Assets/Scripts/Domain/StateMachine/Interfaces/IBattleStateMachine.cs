using Assets.Scripts.Common.Enumerations;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Domain.StateMachine.Interfaces
{
    public interface IBattleStateMachine
    {
        GameState Current { get; }
        UniTask ChangeStateAsync(GameState next);
    }
}
