using System.Linq;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;
using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Implementations.Battle.States
{
    public sealed class GenerateGridState : IGameState
    {
        private readonly IGridsService _gridService;

        public GenerateGridState(IGridsService gridService)
        {
            _gridService = gridService;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            var gridTask = _gridService.OnGridGenerated.FirstAsync();
            _gridService.GenerateGrid(gameSession.GameType);
            var grid = await gridTask;
            gameSession.RoundState.Grid = grid.ToList();
            return GameState.Spawn;
        }

        public void Exit() { }
    }
}
