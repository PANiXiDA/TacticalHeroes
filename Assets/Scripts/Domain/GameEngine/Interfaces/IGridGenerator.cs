using Assets.Scripts.GameEngine.Domain;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using System.Collections.Generic;

namespace Assets.Scripts.GameEngine.Interfaces
{
    public interface IGridGenerator
    {
        List<Tile> GenerateGrid(GameType gameType);
        List<Tile> BaseDeploymentGrid(List<Tile> grid, List<Unit> units, PlayerSide side, int columns);
        List<(int x, int y)> GetDeploymentCoordinates(int minX, int maxX, int minY, int maxY, PlayerSide side, int columns);
    }
}
