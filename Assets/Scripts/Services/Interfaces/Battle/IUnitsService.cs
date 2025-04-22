using System.Collections.Generic;
using Assets.Scripts.Infrastructure.Models;

using R3;
using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IUnitsService
    {
        Observable<IReadOnlyList<Unit>> OnUnitsSpawned { get; }
        void SpawnUnits(List<PlayerBattleData> players);
    }
}
