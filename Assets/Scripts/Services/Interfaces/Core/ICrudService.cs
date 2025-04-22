using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Services.Interfaces.Core
{
    public interface ICrudService<TEntity, TId, TSearchParams, TConvertParams>
        where TEntity : class
        where TId : notnull
        where TSearchParams : class
        where TConvertParams : class, new()
    {
        UniTask<TId> AddOrUpdateAsync(TEntity entity);
        UniTask<IList<TId>> AddOrUpdateAsync(IList<TEntity> entities);
        UniTask<bool> ExistsAsync(TId id);
        UniTask<bool> ExistsAsync(TSearchParams searchParams);
        UniTask<TEntity> GetAsync(TId id, TConvertParams? convertParams = null);
        UniTask<bool> DeleteAsync(TId id);
        UniTask<bool> DeleteAsync(IList<TId> id);
        UniTask<IList<TEntity>> GetAsync(TSearchParams searchParams, TConvertParams convertParams = null);
    }
}
