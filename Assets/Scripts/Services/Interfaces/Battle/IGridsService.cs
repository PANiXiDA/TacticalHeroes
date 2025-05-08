using System;
using System.Collections.Generic;

using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using R3;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IGridsService
    {
        Observable<IReadOnlyList<Tile>> OnGridGenerated { get; }
        void GenerateGrid(GameType type);
        List<Tile> GetGrid();
        Tile GetTile(Guid unitId);
        int GetDistance(Tile a, Tile b);
    }
}
