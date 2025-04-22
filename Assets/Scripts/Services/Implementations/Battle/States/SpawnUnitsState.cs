using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Implementations.Battle.States
{
    public class SpawnUnitsState : IGameState
    {
        private readonly GameSession _gameSession;
        private readonly IUnitsService _unitsService;

        public SpawnUnitsState(
            GameSession gameSession,
            IUnitsService unitsService)
        {
            _gameSession = gameSession;
            _unitsService = unitsService;
        }

        public async UniTask EnterAsync()
        {
            _unitsService.SpawnUnits(_gameSession.Players);
            await UniTask.Yield();
        }

        public void Exit() { }
    }
}
