using System;
using System.Collections.Generic;

using Assets.Scripts.Domain.DTO.Wrappers;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IBattlePreparationsService
    {
        Observable<IReadOnlyList<UnitWrapper>> OnUnitsLoaded { get; }
        UniTask<IReadOnlyList<UnitWrapper>> LoadFromBuildAsync(Guid buildId, PlayerSide side, int teamNumber, List<Tile> grid);
    }
}
