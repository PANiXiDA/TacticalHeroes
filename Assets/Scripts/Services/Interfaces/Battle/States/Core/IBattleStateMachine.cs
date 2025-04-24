using Assets.Scripts.Common.Enumerations;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Interfaces.Battle.States.Core
{
    public interface IBattleStateMachine
    {
        GameState Current { get; }
        UniTask ChangeStateAsync(GameState next);
    }
}
