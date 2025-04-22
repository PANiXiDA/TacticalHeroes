using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Scripts.Repositories.Interfaces.Core
{
    public interface IBaseRepository<TEntity, TId, TSearchParams, TConvertParams>
        where TEntity : class
        where TSearchParams : class
        where TConvertParams : class
    {
        TId AddOrUpdate(TEntity entity);

        IList<TId> AddOrUpdate(IList<TEntity> entity);

        bool Exists(TId id);

        bool Exists(TSearchParams searchParams);

        TEntity Get(TId id, TConvertParams convertParams = null);

        IList<TEntity> Get(TSearchParams searchParams, TConvertParams convertParams = null);

        bool Delete(TId id);
        bool Delete(IList<TId> id);
    }
}
