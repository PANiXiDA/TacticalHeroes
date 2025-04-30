using System;
using System.Collections.Generic;

using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

namespace Assets.Scripts.Domain.DTO.Models
{
    public readonly struct MovementPath
    {
        public Guid UnitId { get; }
        public IReadOnlyList<Tile> Tiles { get; }

        public MovementPath(Guid unitId, IReadOnlyList<Tile> tiles)
        {
            UnitId = unitId;
            Tiles = tiles;
        }
    }
}
