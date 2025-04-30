using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;

using Assets.Scripts.Repositories.Implementations.SQLite.Core;
using Assets.Scripts.Repositories.Interfaces;
using Assets.Scripts.Repositories.Models;

using SQLite4Unity3d;

using Effect = Assets.Scripts.Domain.Entities.Models.Effect;
using DbAbility = Assets.Scripts.Repositories.Models.Ability;
using EntityAbility = Assets.Scripts.Domain.Entities.Models.Ability;

using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Repositories.Implementations.SQLite
{
    public sealed class AbilitiesRepository
        : BaseSQLiteRepository<DbAbility, EntityAbility, int, AbilitiesSearchParams, AbilitiesConvertParams>,
          IAbilitiesRepository
    {
        private readonly IEffectsRepository _effectsRepository;

        public AbilitiesRepository(
            SQLiteContext ctx,
            IEffectsRepository effectsRepository) : base(ctx) 
        {
            _effectsRepository = effectsRepository;
        }

        protected override void MapToDb(EntityAbility entity, DbAbility dbObject)
        {
            dbObject.Type = entity.Type;
        }

        protected override TableQuery<DbAbility> BuildQuery(AbilitiesSearchParams searchParams)
        {
            var dbObjects = _db.Table<DbAbility>();

            if (searchParams.UnitId.HasValue)
            {
                var scopes = _db
                  .Table<UnitAndAbilityScope>()
                  .Where(s => s.UnitId == searchParams.UnitId.Value)
                  .ToList();

                var abilityIds = scopes
                  .Select(s => s.AbilityId)
                  .ToList();

                dbObjects = dbObjects.Where(ability => abilityIds.Contains(ability.Id));
            }
            if (searchParams.HeroId.HasValue)
            {
                var scopes = _db.Table<HeroAndAbilityScope>()
                    .Where(scope => scope.HeroId == searchParams.HeroId.Value)
                    .ToList();

                var abilityIds = scopes
                    .Select(s => s.AbilityId)
                    .ToList();

                dbObjects = dbObjects.Where(ability => abilityIds.Contains(ability.Id));
            }

            return dbObjects;
        }

        protected override EntityAbility MapToEntity(DbAbility dbObject, AbilitiesConvertParams convertParams)
        {
            return new EntityAbility(
                id: dbObject.Id,
                type: dbObject.Type)
            {
                Effects = convertParams.IncludeEffects
                    ? _effectsRepository.Get(
                        new EffectsSearchParams()
                        {
                            AbilityId = dbObject.Id
                        }).ToList()
                    : new List<Effect>()
            };
        }
    }
}
