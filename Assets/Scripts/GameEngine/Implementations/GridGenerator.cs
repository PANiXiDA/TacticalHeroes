using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.GameEngine.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.GameEngine.Implementations
{
    public class GridGenerator : IGridGenerator
    {
        private readonly Random _random;

        public GridGenerator()
        {
            _random = new Random();
        }

        public List<Tile> GenerateGrid(GameType gameType)
        {
            var grid = new List<Tile>();

            var width = 0;
            var height = 10;

            switch (gameType)
            {
                case GameType.Duel:
                    width = 12;
                    break;
                case GameType.Random2x2:
                    width = 24;
                    break;
                case GameType.Team2x2:
                    width = 24;
                    break;
                default:
                    break;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var terrainRoll = _random.Next(0, 50);

                    var terrain = terrainRoll switch
                    {
                        0 => TerrainType.Mountain,
                        1 => TerrainType.Lake,
                        _ => TerrainType.Grass
                    };

                    var tile = new Tile(
                    x: x,
                    y: y,
                    isWalkable: terrain == TerrainType.Grass,
                    terrain: terrain);

                    grid.Add(tile);
                }
            }

            return grid;
        }

        public List<Tile> BaseDeploymentGrid(List<Tile> grid, List<Unit> units, PlayerSide side, int columns)
        {
            var minX = grid.Min(tile => tile.X);
            var maxX = grid.Max(tile => tile.X);
            var minY = grid.Min(tile => tile.Y);
            var maxY = grid.Max(tile => tile.Y);

            var tileLookup = grid.ToDictionary(t => (t.X, t.Y));
            int deployedCount = 0;

            List<(int x, int y)> coordinates = GetDeploymentCoordinates(minX, maxX, minY, maxY, side, columns);

            foreach (var (x, y) in coordinates)
            {
                if (deployedCount >= units.Count)
                    break;

                if (tileLookup.TryGetValue((x, y), out Tile tile) && tile.IsWalkable)
                {
                    tile.OccupiedUnitId = units[deployedCount].Id;
                    tile.IsWalkable = false;
                    deployedCount++;
                }
            }

            return grid;
        }

        /// <summary>
        /// Генерирует список координат для расстановки юнитов.
        /// Для стороны Left начинается с левого верхнего угла (minX, maxY) и движется по рядам сверху вниз,
        /// для стороны Right – с правого нижнего (maxX, minY) и движется снизу вверх.
        /// Количество колонок задается параметром columns.
        /// </summary>
        public List<(int x, int y)> GetDeploymentCoordinates(int minX, int maxX, int minY, int maxY, PlayerSide side, int columns)
        {
            var coordinates = new List<(int x, int y)>();

            if (side == PlayerSide.Left)
            {
                for (int y = maxY; y >= minY; y--)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        coordinates.Add((minX + col, y));
                    }
                }
            }
            else if (side == PlayerSide.Right)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        coordinates.Add((maxX - col, y));
                    }
                }
            }

            return coordinates;
        }
    }
}
