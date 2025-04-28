using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Common.SearchParams;
using Assets.Scripts.Domain.Entities.Models;
using Assets.Scripts.Repositories.Implementations.RealmDB.Core;
using Assets.Scripts.Repositories.Interfaces;
using Assets.Scripts.Repositories.Models.RealmDB;

using DbBuild = Assets.Scripts.Repositories.Models.RealmDB.Build;
using EntityBuild = Assets.Scripts.Domain.Entities.Models.Build;

namespace Assets.Scripts.Repositories.Implementations.RealmDB
{
    public sealed class BuildsRepository 
        : BaseRealmDbRepository<DbBuild, EntityBuild, Guid, BuildsSearchParams, BuildsConvertParams>,
          IBuildsRepository
    {
        private readonly IUnitsRepository _unitsRepository;

        public BuildsRepository(
            RealmContext ctx,
            IUnitsRepository unitsRepository) : base(ctx) 
        {
            _unitsRepository = unitsRepository;
        }

        protected override void MapToDb(EntityBuild entity, DbBuild dbObject)
        {
            dbObject.Name = entity.Name;

            dbObject.Units.Clear();
            foreach (var (id, amount) in entity.UnitIdsAndCounts)
            {
                var slot = new UnitInBuild
                {
                    UnitId = id,
                    Amount = amount
                };
                dbObject.Units.Add(slot);
            }
        }

        protected override IQueryable<DbBuild> BuildQuery(BuildsSearchParams searchParams)
        {
            var dbObjects = _realm.All<DbBuild>().AsQueryable();

            return dbObjects;
        }

        protected override EntityBuild MapToEntity(DbBuild dbObject, BuildsConvertParams convertParams)
        {
            var unitIdsAndCounts = dbObject.Units.ToDictionary(unit => unit.UnitId, unit => unit.Amount);

            return new EntityBuild(
                id: dbObject.Id,
                name: dbObject.Name,
                unitIdsAndCounts: unitIdsAndCounts)
            {
                Units = convertParams.IncludeUnits 
                    ? _unitsRepository.Get(
                        new UnitsSearchParams() 
                        { 
                            Ids = unitIdsAndCounts.Keys.ToList()
                        },
                        new UnitsConvertParams()
                        { 
                            IncludeAbilities = true
                        }).ToList()
                    : new List<Unit>()
            };
        }
    }
}
