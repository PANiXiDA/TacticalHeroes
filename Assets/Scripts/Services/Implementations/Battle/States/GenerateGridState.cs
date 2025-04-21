using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Implementations.Battle.States
{
    public sealed class GenerateGridState : IGameState
    {
        private readonly IGridService _gridService;

        public GenerateGridState(IGridService gridService)
        {
            _gridService = gridService;
        }

        public async UniTask EnterAsync()
        {
            _gridService.GenerateGrid(GameType.Duel);
            await UniTask.Yield();
        }

        public void Exit() { }
    }
}
