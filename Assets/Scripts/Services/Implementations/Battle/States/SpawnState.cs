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

        public async UniTask<GameState?> EnterAsync(GameSession s)
        {
            var gridIndex = s.RoundState.Grid.ToDictionary(t => (t.X, t.Y));

            var tasks = s.Players.Select(p =>
                _battlePreparationsService.LoadFromBuildAsync(p.BuildId, p.Side, p.TeamNumber, s.RoundState.Grid));

            var results = await UniTask.WhenAll(tasks);

            for (int i = 0; i < s.Players.Count; i++)
            {
                var wrappers = results[i];

                s.Players[i].Units = wrappers.Select(w => w.Unit).ToList();

                foreach (var wrapper in wrappers)
                {
                    var tile = gridIndex[(wrapper.TileX, wrapper.TileY)];
                    tile.OccupiedUnitId = wrapper.Unit.Id;
                }
            }

            return null;
        }

        public void Exit() { }
    }
}
