using System;

using Assets.Scripts.GameEngine.DTO.Enums;

namespace Assets.Scripts.GameEngine.DTO.PathFinderCalculator
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }
        public bool IsWalkable { get; set; }
        public TerrainType Terrain { get; }

        public Guid? OccupiedUnitId { get; set; }

        public Tile(
            int x,
            int y,
            bool isWalkable,
            TerrainType terrain)
        {
            X = x;
            Y = y;
            IsWalkable = isWalkable;
            Terrain = terrain;
        }
    }
}
