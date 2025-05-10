using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;

using R3;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattleEndService : IBattleEndService
    {
        private readonly Subject<(string Winners, string Losers)> _battleEndMessageBuilt = new();

        public Observable<(string Winners, string Losers)> OnBattleEndMessageBuilt => _battleEndMessageBuilt.AsObservable();

        public void BattleEnd(List<PlayerBattleData> players)
        {
            var unitsBySide = players
                .GroupBy(player => player.Side)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(player => player.Units?.Sum(unit => unit.Count) ?? 0)
                );

            var loseSide = unitsBySide.First(kv => kv.Value == 0).Key;
            var winSide = unitsBySide.First(kv => kv.Value > 0).Key;

            string winners = string.Join('\n',
                players
                    .Where(player => player.Side == winSide)
                    .Select(player => $"{player.NickName} gets 1000 experience!"));

            string losers = string.Join('\n',
                players
                    .Where(player => player.Side == loseSide)
                    .Select(player => $"{player.NickName} gets 500 experience"));

            _battleEndMessageBuilt.OnNext((winners, losers));
        }
    }
}
