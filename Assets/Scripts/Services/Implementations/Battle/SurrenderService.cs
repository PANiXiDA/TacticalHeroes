using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class SurrenderService : ISurrenderService
    {
        private readonly IMovementsService _movementsService;

        private readonly Subject<List<Guid>> _surrenderUnitsGot = new();

        private List<PlayerBattleData> _playersCache = new();

        public Observable<List<Guid>> OnSurrenderUnitsGot => _surrenderUnitsGot.AsObservable();

        public SurrenderService(IMovementsService movementsService)
        {
            _movementsService = movementsService;
        }

        public void SetPlayersCache(List<PlayerBattleData> players)
        {
            _playersCache = players;
        }

        public UniTask SurrenderAsync(Guid surrenderGameObjectId)
        {
            _movementsService.ClearReachableTiles();

            var surrenderPlayer = _playersCache.FirstOrDefault(player =>
                (player.Hero?.Id == surrenderGameObjectId) ||
                (player.Units?.Any(unit => unit.Id == surrenderGameObjectId) == true));
            if (surrenderPlayer == null)
            {
                return UniTask.CompletedTask;
            }

            var side = surrenderPlayer.Side;
            var surrenderUnits = new List<Unit>();

            foreach (var player in _playersCache.Where(player => player.Side == side))
            {
                foreach (var unit in player.Units)
                {
                    unit.Count = 0;
                    surrenderUnits.Add(unit);
                }
            }

            _surrenderUnitsGot.OnNext(surrenderUnits.Select(unit => unit.Id).ToList());

            return UniTask.CompletedTask;
        }
    }
}
