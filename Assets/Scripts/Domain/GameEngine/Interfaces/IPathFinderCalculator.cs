using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using System.Collections.Generic;

namespace Assets.Scripts.GameEngine.Interfaces
{
    public interface IPathFinderCalculator
    {
        List<Tile> GetPath(PathfindingContext context);
        bool IsReachable(PathfindingContext context);
        List<Tile> GetReachableTiles(PathfindingContext context);
    }
}
