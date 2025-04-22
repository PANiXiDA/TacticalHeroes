using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Repositories.Implementations.SQLite.Core;
using Assets.Scripts.Repositories.Interfaces;
using Assets.Scripts.Repositories.Models;

using SQLite4Unity3d;

using DbEffect = Assets.Scripts.Repositories.Models.Effect;
using EntityEffect = Assets.Scripts.Domain.Entities.Models.Effect;

using System.Linq;

namespace Assets.Scripts.Repositories.Implementations.SQLite
{
    public sealed class EffectsRepository
        : BaseSQLiteRepository<DbEffect, EntityEffect, int, EffectsSearchParams, EffectsConvertParams>,
          IEffectsRepository
    {
        public EffectsRepository(DatabaseContext ctx) : base(ctx) { }

        protected override void MapToDb(EntityEffect entity, DbEffect dbObject)
        {
            dbObject.Type = entity.Type;
            dbObject.Value = entity.Value;
            dbObject.Duration = entity.Duration;
            dbObject.Parameters = entity.Parameters;
        }

        protected override TableQuery<DbEffect> BuildQuery(EffectsSearchParams searchParams)
        {
            var dbObjects = _db.Table<DbEffect>();

            if (searchParams.AbilityId.HasValue)
            {
                var effectIds = _db.Table<AbilityAndEffectScope>()
                    .Where(scope => scope.AbilityId == searchParams.AbilityId.Value)
                    .Select(scope => scope.EffectId)
                    .ToList();

                dbObjects = dbObjects.Where(effect => effectIds.Contains(effect.Id));
            }

            return dbObjects;
        }

        protected override EntityEffect MapToEntity(DbEffect dbObject, EffectsConvertParams convertParams)
        {
            return new EntityEffect(
                id: dbObject.Id,
                type: dbObject.Type,
                value: dbObject.Value,
                duration: dbObject.Duration,
                parameters: dbObject.Parameters);
        }
    }
}
