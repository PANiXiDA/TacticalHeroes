using System.Linq;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;

using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Implementations.Battle.States
{
    public class SpawnState : IGameState
    {
        private readonly IBattlePreparationsService _battlePreparationsService;

        public SpawnState(IBattlePreparationsService battlePreparationsService)
        {
            _battlePreparationsService = battlePreparationsService;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            var gridIndex = gameSession.RoundState.Grid.ToDictionary(tile => (tile.X, tile.Y));

            var tasks = gameSession.Players.Select(player =>
                _battlePreparationsService.LoadFromBuildAsync(
                    player.Id,
                    player.BuildId,
                    player.Side,
                    player.TeamNumber,
                    gameSession.RoundState.Grid));

            var results = await UniTask.WhenAll(tasks);

            for (int i = 0; i < gameSession.Players.Count; i++)
            {
                var wrappers = results[i];

                gameSession.Players[i].Units = wrappers.Select(wrapper => wrapper.Unit).ToList();

                foreach (var wrapper in wrappers)
                {
                    var tile = gridIndex[(wrapper.TileX, wrapper.TileY)];
                    tile.OccupiedUnitId = wrapper.Unit.Id;
                }
            }

            gameSession.RoundState.Units = results.SelectMany(wrappers => wrappers.Select(w => w.Unit)).ToList();

            return GameState.SetATB;
        }

        public void Exit() { }
    }
}
