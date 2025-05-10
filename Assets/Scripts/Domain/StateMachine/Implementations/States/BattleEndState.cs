using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces.States;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Domain.StateMachine.Implementations.States
{
    public sealed class BattleEndState : IGameState
    {
        private readonly IBattleEndService _battleEndService;

        public BattleEndState(IBattleEndService battleEndService) 
        {
            _battleEndService = battleEndService;
        }

        public UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            _battleEndService.BattleEnd(gameSession.Players);
            return UniTask.FromResult<GameState?>(null);
        }

        public void Exit() { }
    }
}
