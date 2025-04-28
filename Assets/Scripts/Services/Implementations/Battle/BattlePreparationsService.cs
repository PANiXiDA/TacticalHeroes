using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.ConvertParams;
using Assets.Scripts.Domain.DTO.Wrappers;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.Domain.Entities.Models.Unit;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class BattlePreparationsService : IBattlePreparationsService
    {
        private readonly IBuildsService _buildsService;

        private readonly Subject<IReadOnlyList<UnitWrapper>> _unitsLoaded = new();

        public Observable<IReadOnlyList<UnitWrapper>> OnUnitsLoaded => _unitsLoaded.AsObservable();

        public BattlePreparationsService(IBuildsService buildsService)
        {
            _buildsService = buildsService;
        }

        public async UniTask<IReadOnlyList<UnitWrapper>> LoadFromBuildAsync(
            int? playerId,
            Guid buildId,
            PlayerSide side,
            int teamNumber,
            List<Tile> grid)
        {
            var build = await _buildsService.GetAsync(
                buildId,
                new BuildsConvertParams()
                {
                    IncludeUnits = true
                });

            var unitWrappers = new List<UnitWrapper>();
            var occupiedTiles = new List<Tile>();

            foreach(var unit in build.Units)
            {
                var tile = GetFreeTile(grid, occupiedTiles, side);
                occupiedTiles.Add(tile);

                var unitWrapper = new UnitWrapper(
                    side: side,
                    teamNumber: teamNumber,
                    tileX: tile.X,
                    tileY: tile.Y);
                unitWrapper.Unit = Unit.MapToDomain(unit);
                unitWrapper.Unit.OwnerId = playerId;
                unitWrapper.Unit.Count = build.UnitIdsAndCounts[unit.Id];
                unitWrappers.Add(unitWrapper);
            }

            _unitsLoaded.OnNext(unitWrappers);
            return unitWrappers;
        }

        private Tile GetFreeTile(List<Tile> grid, List<Tile> occupiedTiles, PlayerSide side)
        {
            var candidates = grid
                .Where(tile =>
                    !occupiedTiles.Contains(tile) 
                    && tile.OccupiedUnitId == null 
                    && tile.IsWalkable 
                    && (side == PlayerSide.Left ? tile.X < 2 : tile.X >= grid.Max(c => c.X) - 1))
                .ToList();

            return candidates[Random.Range(0, candidates.Count)];
        }
    }
}
