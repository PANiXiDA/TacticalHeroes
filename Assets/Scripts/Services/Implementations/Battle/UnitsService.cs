using System.Collections.Generic;

using Assets.Scripts.Services.Interfaces.Battle;
using Unit = Assets.Scripts.GameEngine.Domain.Unit;

using R3;
using Assets.Scripts.Infrastructure.Models;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class UnitsService : IUnitsService
    {
        private readonly Subject<IReadOnlyList<Unit>> _unitsSpawned = new();

        public Observable<IReadOnlyList<Unit>> OnUnitsSpawned => _unitsSpawned.AsObservable();

        public UnitsService()
        {
        }

        public void SpawnUnits(List<PlayerBattleData> players)
        {

        }
    }
}
