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
        private readonly IMovementsService _movementsService;
        private readonly IAttacksService _attacksService;

        public WaitActionState(
            IMovementsService movementsService,
            IAttacksService attacksService)
        {
            _movementsService = movementsService;
            _attacksService = attacksService;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
           return await Observable.Merge(
                _movementsService.OnPathComputed.Select(_ => GameState.ApplyAction),
                _attacksService.OnAttackDone.Select(_ => GameState.ApplyAction))
                .FirstAsync();
        }

        public void Exit() { }
    }
}
