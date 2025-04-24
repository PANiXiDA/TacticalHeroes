using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Realms;
using Assets.Scripts.Repositories.Interfaces.Core;
using MongoDB.Bson;

namespace Assets.Scripts.Repositories.Implementations.RealmDB.Core
{
    public abstract class BaseRealmDbRepository<TDbObject, TEntity, TId, TSearchParams, TConvertParams>
        : IBaseRepository<TEntity, TId, TSearchParams, TConvertParams>
        where TDbObject : RealmObject, new()
        where TEntity : class
        where TId : notnull
        where TSearchParams : class
        where TConvertParams : class, new()
    {
        protected readonly Realm _realm;

        private readonly Func<TEntity, TId> _getEntityId;
        private readonly Func<TDbObject, TId> _getDbObjectId;

        private readonly Action<TEntity, TId> _setEntityId;
        private readonly Action<TDbObject, TId> _setDbObjectId;

        protected virtual bool RequiresRelationUpdates => false;

        protected BaseRealmDbRepository(RealmContext ctx)
        {
            _realm = ctx.Connection ?? throw new ArgumentNullException(nameof(ctx));

            _getEntityId = IdGetter<TEntity>();
            _getDbObjectId = IdGetter<TDbObject>();

            _setEntityId = IdSetter<TEntity>();
            _setDbObjectId = IdSetter<TDbObject>();
        }

        public TId AddOrUpdate(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _realm.Write(() =>
            {
                var id = GetEntityId(entity);

                if (typeof(TId) == typeof(int) && EqualityComparer<TId>.Default.Equals(id, default))
                {
                    var nextId = (_realm.All<TDbObject>()
                        .AsQueryable()
                        .Select(o => (int)(object)_getDbObjectId(o))
                        .DefaultIfEmpty(0)
                        .Max()) + 1;

                    _setEntityId(entity, (TId)(object)nextId);
                    id = (TId)(object)nextId;
                }
                else if (typeof(TId) == typeof(Guid) && EqualityComparer<TId>.Default.Equals(id, default))
                {
                    id = (TId)(object)Guid.NewGuid();
                    _setEntityId(entity, id);
                }
                else if (typeof(TId) == typeof(ObjectId) && EqualityComparer<TId>.Default.Equals(id, default))
                {
                    id = (TId)(object)ObjectId.GenerateNewId();
                    _setEntityId(entity, id);
                }

                var dbObj = new TDbObject();
                MapToDb(entity, dbObj);
                _setDbObjectId(dbObj, id);
                _realm.Add(dbObj, update: true);

                if (RequiresRelationUpdates)
                {
                    UpdateRelations(entity, dbObj);
                }
            });

            return GetEntityId(entity);
        }

        public IList<TId> AddOrUpdate(IList<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            var ids = new List<TId>(entities.Count);

            _realm.Write(() =>
            {
                foreach (var entity in entities)
                    ids.Add(AddOrUpdate(entity));
            });

            return ids;
        }

        public bool Delete(TId id)
        {
            var obj = FindByPk(id);
            if (obj == null) return false;

            _realm.Write(() => _realm.Remove(obj));
            return true;
        }

        public bool Delete(IList<TId> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            var removed = false;
            _realm.Write(() =>
            {
                foreach (var id in ids)
                {
                    var obj = FindByPk(id);
                    if (obj != null)
                    {
                        _realm.Remove(obj);
                        removed = true;
                    }
                }
            });

            return removed;
        }

        public TEntity Get(TId id, TConvertParams convertParams = null)
        {
            convertParams ??= new TConvertParams();
            var dbObject = FindByPk(id) ?? throw new InvalidOperationException($"Объект {typeof(TDbObject).Name} c PK={id} не найден");

            return MapToEntity(dbObject, convertParams);
        }

        public IList<TEntity> Get(TSearchParams searchParams, TConvertParams convertParams = null)
        {
            convertParams ??= new TConvertParams();
            var list = BuildQuery(searchParams).ToList();
            return list.Select(db => MapToEntity(db, convertParams)).ToList();
        }

        public bool Exists(TId id) => FindByPk(id) != null;

        public bool Exists(TSearchParams searchParams) => BuildQuery(searchParams).Any();

        protected virtual TId GetEntityId(TEntity entity) => _getEntityId(entity);
        protected virtual TId GetDbObjectId(TDbObject dbObject) => _getDbObjectId(dbObject);

        protected abstract void MapToDb(TEntity entity, TDbObject dbObject);
        protected abstract TEntity MapToEntity(TDbObject dbObject, TConvertParams convertParams);
        protected abstract IQueryable<TDbObject> BuildQuery(TSearchParams searchParams);
        protected virtual void UpdateRelations(TEntity entity, TDbObject dbObject) { }

        protected TDbObject FindByPk(TId id)
        {
            object boxed = id!;

            return boxed switch
            {
                string s => _realm.Find<TDbObject>(s),
                long l => _realm.Find<TDbObject>((long?)l),
                int i => _realm.Find<TDbObject>((long?)(long)i),
                Guid g => _realm.Find<TDbObject>((Guid?)g),
                ObjectId oid => _realm.Find<TDbObject>((ObjectId?)oid),
                _ => throw new NotSupportedException($"Тип PK {typeof(TId).Name} не поддерживается Realm")
            };
        }

        private static Func<T, TId> IdGetter<T>()
        {
            var type = typeof(T);
            var pi = type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                    ?? type.GetProperty("Id")
                    ?? throw new InvalidOperationException($"{type.Name} не содержит PK-свойства");

            var param = Expression.Parameter(type, "x");
            var body = Expression.Convert(Expression.Property(param, pi), typeof(TId));
            return Expression.Lambda<Func<T, TId>>(body, param).Compile();
        }

        private static Action<T, TId> IdSetter<T>()
        {
            var type = typeof(T);
            var pi = type.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                    ?? type.GetProperty("Id")
                    ?? throw new InvalidOperationException($"{type.Name} не содержит PK-свойства");

            var obj = Expression.Parameter(type, "o");
            var val = Expression.Parameter(typeof(TId), "v");
            var body = Expression.Assign(Expression.Property(obj, pi), Expression.Convert(val, pi.PropertyType));

            return Expression.Lambda<Action<T, TId>>(body, obj, val).Compile();
        }
    }
}
