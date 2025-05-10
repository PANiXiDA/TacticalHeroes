using System.Collections.Generic;

using Assets.Scripts.Infrastructure.Models;

using R3;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IBattleEndService
    {
        Observable<(string Winners, string Losers)> OnBattleEndMessageBuilt { get; }
        void BattleEnd(List<PlayerBattleData> players);
    }
}
