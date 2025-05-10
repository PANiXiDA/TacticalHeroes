using System;
using System.Collections.Generic;

using Assets.Scripts.Infrastructure.Models;

using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface ISurrenderService
    {
        Observable<List<Guid>> OnSurrenderUnitsGot { get; }
        void SetPlayersCache(List<PlayerBattleData> players);
        UniTask SurrenderAsync(Guid surrenderGameObjectId);
    }
}
