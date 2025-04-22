using System.Collections.Generic;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Core;

using R3;

using DomainUnit = Assets.Scripts.GameEngine.Domain.Unit;
using EntityUnit = Assets.Scripts.Domain.Entities.Models.Unit;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IUnitsService : ICrudService<EntityUnit, int, UnitsSearchParams, UnitsConvertParams>
    {
        Observable<IReadOnlyList<DomainUnit>> OnUnitsSpawned { get; }
        void SpawnUnits(List<PlayerBattleData> players);
    }
}
