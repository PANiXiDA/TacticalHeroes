using System.Collections.Generic;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Repositories.Implementations.SQLite.Core;
using Assets.Scripts.Repositories.Interfaces;
using SQLite4Unity3d;

using Ability = Assets.Scripts.Domain.Entities.Models.Ability;
using DbHero = Assets.Scripts.Repositories.Models.Hero;
using EntityHero = Assets.Scripts.Domain.Entities.Models.Hero;

using System.Linq;

namespace Assets.Scripts.Repositories.Implementations.SQLite
{
    public sealed class HeroesRepository
        : BaseSQLiteRepository<DbHero, EntityHero, int, HeroesSearchParams, HeroesConvertParams>,
          IHeroesRepository
    {
        private readonly IAbilitiesRepository _abilitiesRepository;

        public HeroesRepository(
            DatabaseContext ctx,
            IAbilitiesRepository abilitiesRepository) : base(ctx) 
        {
            _abilitiesRepository = abilitiesRepository;
        }

        protected override void MapToDb(EntityHero entity, DbHero dbObject)
        {
            dbObject.Attack = entity.Attack;
            dbObject.Defence = entity.Defence;
            dbObject.MinDamage = entity.MinDamage;
            dbObject.MaxDamage = entity.MaxDamage;
            dbObject.Initiative = entity.Initiative;
            dbObject.Morale = entity.Morale;
            dbObject.Luck = entity.Luck;
            dbObject.Name = entity.Name;
            dbObject.Description = entity.Description;
        }

        protected override TableQuery<DbHero> BuildQuery(HeroesSearchParams searchParams)
        {
            var dbObjects = _db.Table<DbHero>();

            return dbObjects;
        }

        protected override EntityHero MapToEntity(DbHero dbObject, HeroesConvertParams convertParams)
        {
            return new EntityHero(
                id: dbObject.Id,
                attack: dbObject.Attack,
                defence: dbObject.Defence,
                minDamage: dbObject.MinDamage,
                maxDamage: dbObject.MaxDamage,
                initiative: dbObject.Initiative,
                morale: dbObject.Morale,
                luck: dbObject.Luck,
                name: dbObject.Name,
                description: dbObject.Description)
            {
                Abilities = convertParams.IncludeAbilities
                    ? _abilitiesRepository.Get(
                        new AbilitiesSearchParams()
                        {
                            HeroId = dbObject.Id
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
