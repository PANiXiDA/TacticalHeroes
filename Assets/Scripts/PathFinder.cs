using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Enumeration;
using Unity.Mathematics;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance;
    public List<Vector2> PathToTarget;
    List<Tile> CheckedNodes;
    List<Tile> WaitingNodes;
    private void Awake()
    {
        Instance = this;
    }
    public List<Vector2> GetPath(Vector2 start, Vector2 target, BaseUnit unit)
    {
        PathToTarget = new List<Vector2>();
        CheckedNodes = new List<Tile>();
        WaitingNodes = new List<Tile>();

        Vector2 StartPosition = start;
        Vector2 TargetPosition = target;

        if (StartPosition == TargetPosition) return PathToTarget;
        Tile startNode = GridManager.Instance.GetTileAtPosition(start);
        CalculateHeuristic(startNode, 0, StartPosition, TargetPosition, null);
        WaitingNodes.Add(startNode);

        while (WaitingNodes.Count > 0)
        {
            Tile nodeToCheck = WaitingNodes.Where(x => x.F == WaitingNodes.Min(y => y.F)).FirstOrDefault(); 
                                                                                                                                                                                                      
            if (nodeToCheck.Position == TargetPosition)
            {
                return CalculatePathFromNode(nodeToCheck);
            }
            var walkable = nodeToCheck.Walkable;
            if ((!walkable && !unit.abilities.Contains(Ability.Fly)) && nodeToCheck.Position != StartPosition)
            {
                WaitingNodes.Remove(nodeToCheck);
                CheckedNodes.Add(nodeToCheck);
            }
            else
            {
                WaitingNodes.Remove(nodeToCheck);
                CheckedNodes.Add(nodeToCheck);
                WaitingNodes.AddRange(GetNeighbourNodes(nodeToCheck, unit));
            }
        }
        return PathToTarget;
    }
    public List<Vector2> CalculatePathFromNode(Tile node)
    {
        var path = new List<Vector2>();
        Tile currentNode = node;
        path.Add(new Vector2(currentNode.Position.x, currentNode.Position.y));
        while (currentNode.PreviousNode != null)
        {
            path.Add(new Vector2(currentNode.PreviousNode.Position.x, currentNode.PreviousNode.Position.y));
            currentNode = currentNode.PreviousNode;
        }
        return path;
    }
    List<Tile> GetNeighbourNodes(Tile node, BaseUnit unit)
    {
        var Neighbours = new List<Tile>();
        var directions = new Vector2Int[] {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1)
        };
        foreach (var direction in directions)
        {
            var neighbourPosition = new Vector2(node.Position.x + direction.x, node.Position.y + direction.y);
            var neighbour = GridManager.Instance.GetTileAtPosition(neighbourPosition);

            if (neighbour != null && IsNeighbourValid(node, neighbour, unit, direction, neighbourPosition))
            {
                Neighbours.Add(neighbour);
            }
        }
        return Neighbours;
    }

    private bool IsNeighbourValid(Tile node, Tile neighbour, BaseUnit unit, Vector2Int direction, Vector2 neighbourPosition)
    {
        float distance = direction.x == 0 || direction.y == 0 ? 1 : math.sqrt(2);
        if (neighbour != null && (neighbour.Walkable || unit.abilities.Contains(Ability.Fly))
            && (!CheckedNodes.Contains(neighbour) || node.G + distance < neighbour.G))
        {
            CalculateHeuristic(neighbour, node.G + distance, neighbourPosition, node.TargetPosition, node);
            if (!WaitingNodes.Contains(neighbour))
                return true;
        }

        return false;
    }
    public void CalculateHeuristic(Tile node, float g, Vector2 nodePosition, Vector2 targetPosition, Tile previousNode)
    {
        node.Position = nodePosition;
        node.TargetPosition = targetPosition;
        node.PreviousNode = previousNode;
        node.G = g;

        float dx = math.abs(node.TargetPosition.x - node.Position.x);
        float dy = math.abs(node.TargetPosition.y - node.Position.y);
        
        node.H = math.sqrt(2) * math.min(dx, dy) + math.max(dx, dy) - math.min(dx, dy);

        node.F = node.G + node.H;
    }
}