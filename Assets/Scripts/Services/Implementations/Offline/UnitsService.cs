using System.Collections.Generic;

using Assets.Scripts.Services.Interfaces.Battle;

using Assets.Scripts.Repositories.Interfaces;
using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Domain.Entities.Models;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class UnitsService : IUnitsService
    {
        private readonly IUnitsRepository _unitsRepository;

        public UnitsService(IUnitsRepository unitsRepository)
        {
            _unitsRepository = unitsRepository;
        }

        public UniTask<int> AddOrUpdateAsync(Unit entity)
        {
            var id = _unitsRepository.AddOrUpdate(entity);
            return UniTask.FromResult(id);
        }

        public UniTask<IList<int>> AddOrUpdateAsync(IList<Unit> entities)
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

        public UniTask<Unit> GetAsync(int id, UnitsConvertParams convertParams = null)
        {
            var unit = _unitsRepository.Get(id, convertParams);
            return UniTask.FromResult(unit);
        }

        public UniTask<IList<Unit>> GetAsync(UnitsSearchParams searchParams, UnitsConvertParams convertParams = null)
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
    }
}
