using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Repositories.Interfaces;
using System.Collections.Generic;

using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using Assets.Scripts.Domain.Entities.Models;
using System;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BuildsService : IBuildsService
    {
        private readonly IBuildsRepository _buildsRepository;

        public BuildsService(IBuildsRepository buildsRepository)
        {
            _buildsRepository = buildsRepository;
        }

        public UniTask<Guid> AddOrUpdateAsync(Build entity)
        {
            var id = _buildsRepository.AddOrUpdate(entity);
            return UniTask.FromResult(id);
        }

        public UniTask<IList<Guid>> AddOrUpdateAsync(IList<Build> entities)
        {
            var ids = _buildsRepository.AddOrUpdate(entities);
            return UniTask.FromResult(ids);
        }

        public UniTask<bool> ExistsAsync(Guid id)
        {
            var exists = _buildsRepository.Exists(id);
            return UniTask.FromResult(exists);
        }

        public UniTask<bool> ExistsAsync(BuildsSearchParams searchParams)
        {
            var exists = _buildsRepository.Exists(searchParams);
            return UniTask.FromResult(exists);
        }

        public UniTask<Build> GetAsync(Guid id, BuildsConvertParams convertParams = null)
        {
            var unit = _buildsRepository.Get(id, convertParams);
            return UniTask.FromResult(unit);
        }

        public UniTask<IList<Build>> GetAsync(BuildsSearchParams searchParams, BuildsConvertParams convertParams = null)
        {
            var entities = _buildsRepository.Get(searchParams, convertParams);
            return UniTask.FromResult(entities);
        }

        public UniTask<bool> DeleteAsync(Guid id)
        {
            var deleted = _buildsRepository.Delete(id);
            return UniTask.FromResult(deleted);
        }

        public UniTask<bool> DeleteAsync(IList<Guid> ids)
        {
            var deleted = _buildsRepository.Delete(ids);
            return UniTask.FromResult(deleted);
        }
    }
}
