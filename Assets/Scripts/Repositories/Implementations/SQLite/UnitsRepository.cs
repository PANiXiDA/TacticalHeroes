using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Repositories.Implementations.SQLite.Core;
using Assets.Scripts.Repositories.Interfaces;

using SQLite4Unity3d;

using Ability = Assets.Scripts.Domain.Entities.Models.Ability;
using DbUnit = Assets.Scripts.Repositories.Models.Unit;
using EntityUnit = Assets.Scripts.Domain.Entities.Models.Unit;

namespace Assets.Scripts.Repositories.Implementations.SQLite
{
    public sealed class UnitsRepository
        : BaseSQLiteRepository<DbUnit, EntityUnit, int, UnitsSearchParams, UnitsConvertParams>,
          IUnitsRepository
    {
        private readonly IAbilitiesRepository _abilitiesRepository;

        public UnitsRepository(
            SQLiteContext ctx,
            IAbilitiesRepository abilitiesRepository) : base(ctx)
        {
            _abilitiesRepository = abilitiesRepository;
        }

        protected override void MapToDb(EntityUnit entity, DbUnit dbObject)
        {
            dbObject.Attack = entity.Attack;
            dbObject.Defence = entity.Defence;
            dbObject.MinDamage = entity.MinDamage;
            dbObject.MaxDamage = entity.MaxDamage;
            dbObject.Initiative = entity.Initiative;
            dbObject.Morale = entity.Morale;
            dbObject.Luck = entity.Luck;
            dbObject.Health = entity.Health;
            dbObject.Speed = entity.Speed;
            dbObject.Range = entity.Range;
            dbObject.Arrows = entity.Arrows;
            dbObject.Name = entity.Name;
            dbObject.Description = entity.Description;
        }

        protected override TableQuery<DbUnit> BuildQuery(UnitsSearchParams searchParams)
        {
            var dbObjects = _db.Table<DbUnit>();

            if (searchParams.Ids.Any())
            {
                dbObjects = dbObjects.Where(u => searchParams.Ids.Contains(u.Id));
            }

            return dbObjects;
        }

        protected override EntityUnit MapToEntity(DbUnit dbObject, UnitsConvertParams convertParams)
        {
            return new EntityUnit(
                id: dbObject.Id,
                attack: dbObject.Attack,
                defence: dbObject.Defence,
                minDamage: dbObject.MinDamage,
                maxDamage: dbObject.MaxDamage,
                initiative: dbObject.Initiative,
                morale: dbObject.Morale,
                luck: dbObject.Luck,
                health: dbObject.Health,
                speed: dbObject.Speed,
                range: dbObject.Range,
                arrows: dbObject.Arrows,
                name: dbObject.Name,
                description: dbObject.Description)
            {
                Abilities = convertParams.IncludeAbilities
                    ? _abilitiesRepository.Get(
                        new AbilitiesSearchParams()
                        {
                            UnitId = dbObject.Id
                        },
                        new AbilitiesConvertParams()
                        {
                            IncludeEffects = true
                        }).ToList()
                    : new List<Ability>()
            };
        }
    }
}
