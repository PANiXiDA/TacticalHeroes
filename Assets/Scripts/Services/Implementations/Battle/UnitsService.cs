using System.Collections.Generic;

using Assets.Scripts.Services.Interfaces.Battle;

using DomainUnit = Assets.Scripts.GameEngine.Domain.Unit;
using EntityUnit = Assets.Scripts.Domain.Entities.Models.Unit;

using R3;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Repositories.Interfaces;
using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class UnitsService : IUnitsService
    {
        private readonly Subject<IReadOnlyList<DomainUnit>> _unitsSpawned = new();

        private readonly IUnitsRepository _unitsRepository;

        public Observable<IReadOnlyList<DomainUnit>> OnUnitsSpawned => _unitsSpawned.AsObservable();

        public UnitsService(IUnitsRepository unitsRepository)
        {
            _unitsRepository = unitsRepository;
        }

        public UniTask<int> AddOrUpdateAsync(EntityUnit entity)
        {
            var id = _unitsRepository.AddOrUpdate(entity);
            return UniTask.FromResult(id);
        }

        public UniTask<IList<int>> AddOrUpdateAsync(IList<EntityUnit> entities)
        {
            var ids = _unitsRepository.AddOrUpdate(entities);
            return UniTask.FromResult(ids);
        }

        public UniTask<bool> ExistsAsync(int id)
        {
            var exists = _unitsRepository.Exists(id);
            return UniTask.FromResult(exists);
        }

        public UniTask<bool> ExistsAsync(UnitsSearchParams searchParams)
        {
            var exists = _unitsRepository.Exists(searchParams);
            return UniTask.FromResult(exists);
        }

        public UniTask<EntityUnit> GetAsync(int id, UnitsConvertParams convertParams = null)
        {
            var unit = _unitsRepository.Get(id, convertParams);
            return UniTask.FromResult(unit);
        }

        public UniTask<IList<EntityUnit>> GetAsync(UnitsSearchParams searchParams, UnitsConvertParams convertParams = null)
        {
            var entities = _unitsRepository.Get(searchParams, convertParams);
            return UniTask.FromResult(entities);
        }

        public UniTask<bool> DeleteAsync(int id)
        {
            var deleted = _unitsRepository.Delete(id);
            return UniTask.FromResult(deleted);
        }

        public UniTask<bool> DeleteAsync(IList<int> ids)
        {
            var deleted = _unitsRepository.Delete(ids);
            return UniTask.FromResult(deleted);
        }

        public void SpawnUnits(List<PlayerBattleData> players)
        {

        }
    }
}
