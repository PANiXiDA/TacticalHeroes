using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;

using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Implementations.Battle.States
{
    public sealed class WaitActionState : IGameState
    {
        private readonly IBattleActionsFacade _battleActionsFacade;

        public WaitActionState(IBattleActionsFacade battleActionsFacade)
        {
            _battleActionsFacade = battleActionsFacade;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            return await Observable.Merge(
                    _battleActionsFacade.OnMovementCompleted.Do(_ =>
                        gameSession.RoundState.LastCommand = CommandType.Move)
                        .Select(_ => GameState.ApplyAction),

                    _battleActionsFacade.OnAttackDone.Do(_ =>
                        gameSession.RoundState.LastCommand = CommandType.Attack)
                        .Select(_ => GameState.ApplyAction),

                    _battleActionsFacade.OnDefenceDone.Do(_ =>
                        gameSession.RoundState.LastCommand = CommandType.Defence)
                        .Select(_ => GameState.ApplyAction),

                    _battleActionsFacade.OnWaitDone.Do(_ =>
                        gameSession.RoundState.LastCommand = CommandType.Wait)
                        .Select(_ => GameState.ApplyAction))
                .FirstAsync();
        }

        public void Exit() { }
    }
}
