using System.Collections.Generic;
using System.Linq;
using System;
using SQLite4Unity3d;
using System.Linq.Expressions;
using System.Reflection;

namespace Assets.Scripts.Repositories.Implementations.SQLite.Core
{
    public abstract class BaseSQLiteRepository<TDbObject, TEntity, TId, TSearchParams, TConvertParams>
        where TDbObject : class, new()
        where TEntity : class
        where TId : notnull
        where TSearchParams : class
        where TConvertParams : class, new()
    {
        protected readonly SQLiteConnection _db;

        protected virtual bool RequiresRelationUpdates => false;

        private readonly Func<TEntity, TId> _getEntityId;
        private readonly Func<TDbObject, TId> _getDbObjectId;

        protected BaseSQLiteRepository(DatabaseContext ctx)
        {
            _db = ctx.Connection ?? throw new ArgumentNullException(nameof(ctx));
            _getEntityId = IdGetter<TEntity>();
            _getDbObjectId = IdGetter<TDbObject>();
        }

        public TId AddOrUpdate(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var id = GetEntityId(entity);
            var dbObject = _db.Find<TDbObject>(id);
            var exists = dbObject != null;
            dbObject ??= new TDbObject();

            MapToDb(entity, dbObject);

            if (exists)
            {
                _db.Update(dbObject);
            }
            else
            {
                _db.Insert(dbObject);
            }

            if (RequiresRelationUpdates)
            {
                UpdateRelations(entity, dbObject);
            }

            return GetDbObjectId(dbObject);
        }

        public IList<TId> AddOrUpdate(IList<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            var ids = new List<TId>();
            _db.RunInTransaction(() =>
            {
                foreach (var entity in entities)
                {
                    ids.Add(AddOrUpdate(entity));
                }
            });

            return ids;
        }

        public bool Delete(TId id) => _db.Delete<TDbObject>(id) > 0;

        public bool Delete(IList<TId> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            var count = 0;
            _db.RunInTransaction(() =>
            {
                foreach (var id in ids)
                {
                    if (_db.Delete<TDbObject>(id) > 0)
                    {
                        count++;
                    }
                }
            });

            return count > 0;
        }

        public TEntity Get(TId id, TConvertParams convertParams = null)
        {
            convertParams = convertParams ?? new TConvertParams();

            var dbObject = _db.Find<TDbObject>(id);
            if (dbObject == null) throw new ArgumentNullException(nameof(dbObject));

            return MapToEntity(dbObject, convertParams);
        }

        public IList<TEntity> Get(TSearchParams searchParams, TConvertParams convertParams = null)
        {
            convertParams = convertParams ?? new TConvertParams();

            var dbObjects = BuildQuery(searchParams).ToList();

            var result = dbObjects.Select(dbObject => MapToEntity(dbObject, convertParams)).ToList();

            return result;
        }

        public bool Exists(TId id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var exists = _db.Find<TDbObject>(id) != null;
            return exists;
        }

        public bool Exists(TSearchParams searchParams)
        {
            if (searchParams == null) throw new ArgumentNullException(nameof(searchParams));
            var exists = BuildQuery(searchParams).Any();
            return exists;
        }

        protected virtual TId GetEntityId(TEntity entity) => _getEntityId(entity);
        protected virtual TId GetDbObjectId(TDbObject dbObject) => _getDbObjectId(dbObject);

        protected abstract void MapToDb(TEntity entity, TDbObject dbObject);
        protected abstract TEntity MapToEntity(TDbObject dbObject, TConvertParams convertParams);

        protected abstract TableQuery<TDbObject> BuildQuery(TSearchParams searchParams);

        protected virtual void UpdateRelations(TEntity entity, TDbObject dbObject) { }

        private static Func<T, TId> IdGetter<T>()
        {
            var type = typeof(T);

            var pi = type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                ?? type.GetProperty("Id")
                ?? throw new InvalidOperationException($"{type.Name} не содержит PK-свойства");

            var param = Expression.Parameter(type, "x");
            var body = Expression.Convert(Expression.Property(param, pi), typeof(TId));
            var lambda = Expression.Lambda<Func<T, TId>>(body, param);

            return lambda.Compile();
        }
    }
}
