using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;

using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Implementations.Battle.States
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
