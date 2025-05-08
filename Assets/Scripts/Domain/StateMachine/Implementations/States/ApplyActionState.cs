using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces.States;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Domain.StateMachine.Implementations.States
{
    public sealed class ApplyActionState : IGameState
    {
        private readonly IBattleTurnsService _battleTurnsService;

        public ApplyActionState(IBattleTurnsService battleTurnsService)
        {
            _battleTurnsService = battleTurnsService;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            var turnEndTask = _battleTurnsService.OnTurnEnded.FirstAsync();
            _battleTurnsService.CompleteTurnAsync(gameSession.RoundState, gameSession.GameHistory).Forget();
            await turnEndTask;

            return GameState.CheckBattleEnd;
        }

        public void Exit() { }
    }
}
