using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using Assets.Scripts.Enumeration;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance;
    public List<Vector2> PathToTarget;
    List<Tile> CheckedNodes = new List<Tile>();
    List<Tile> WaitingNodes = new List<Tile>();
    BaseUnit Unit;
    private void Awake()
    {
        Instance = this;
    }
    public List<Vector2> GetPath(Vector2 start, Vector2 target, BaseUnit unit)
    {
        PathToTarget = new List<Vector2>();
        CheckedNodes = new List<Tile>();
        WaitingNodes = new List<Tile>();
        Unit = unit;

        Vector2 StartPosition = start;
        Vector2 TargetPosition = target;

        if (StartPosition == TargetPosition) return PathToTarget;

        Tile startNode = GridManager.Instance.GetTileAtPosition(start);
        startNode.SetTile(0, StartPosition, TargetPosition, null);

        WaitingNodes.Add(startNode);

        while (WaitingNodes.Count > 0)
        {
            Tile nodeToCheck = WaitingNodes.Where(x => x.F == WaitingNodes.Min(y => y.F)).FirstOrDefault(); //из всего списка ожидающих плиток
                                                                                                            //выбрали с самым минимальным F                                                                                              
            if (nodeToCheck.Position == TargetPosition)
            {
                return CalculatePathFromNode(nodeToCheck);
            }
            var walkable = nodeToCheck.Walkable;
            if ((!walkable && !Unit.abilities.Contains(Abilities.Fly))  && nodeToCheck.Position != StartPosition)
            {
                WaitingNodes.Remove(nodeToCheck);
                CheckedNodes.Add(nodeToCheck);
            }
            else
            {
                WaitingNodes.Remove(nodeToCheck);
                CheckedNodes.Add(nodeToCheck);
                WaitingNodes.AddRange(GetNeighbourNodes(nodeToCheck));
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
    List<Tile> GetNeighbourNodes(Tile node)
    {
        var Neighbours = new List<Tile>();

        Tile node1 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x + 1, node.Position.y));

        if (node1 != null && (node1.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node1) || node.G + 1 < node1.G))
        {
            node1.SetTile(node.G + 1, new Vector2(node.Position.x + 1, node.Position.y), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node1))
                Neighbours.Add(node1);
        }

        Tile node2 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x, node.Position.y + 1));

        if (node2 != null && (node2.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node2) || node.G + 1 < node2.G))
        {
            node2.SetTile(node.G + 1, new Vector2(node.Position.x, node.Position.y + 1), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node2))
                Neighbours.Add(node2);
        }

        Tile node3 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x, node.Position.y - 1));

        if (node3 != null && (node3.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node3) || node.G + 1 < node3.G))
        {
            node3.SetTile(node.G + 1, new Vector2(node.Position.x, node.Position.y - 1), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node3))
                Neighbours.Add(node3);
        }

        Tile node4 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x - 1, node.Position.y));

        if (node4 != null && (node4.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node4) || node.G + 1 < node4.G))
        {
            node4.SetTile(node.G + 1, new Vector2(node.Position.x - 1, node.Position.y), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node4))
                Neighbours.Add(node4);
        }

        Tile node5 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x + 1, node.Position.y + 1));

        if (node5 != null && (node5.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node5) || node.G + (float)Math.Sqrt(2) < node5.G))
        {
            node5.SetTile(node.G + (float)Math.Sqrt(2), new Vector2(node.Position.x + 1, node.Position.y + 1), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node5))
                Neighbours.Add(node5);
        }

        Tile node6 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x + 1, node.Position.y - 1));

        if (node6 != null && (node6.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node6) || node.G + (float)Math.Sqrt(2) < node6.G))
        {
            node6.SetTile(node.G + (float)Math.Sqrt(2), new Vector2(node.Position.x + 1, node.Position.y - 1), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node6))
                Neighbours.Add(node6);
        }

        Tile node7 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x - 1, node.Position.y - 1));

        if (node7 != null && (node7.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node7) || node.G + (float)Math.Sqrt(2) < node7.G))
        {
            node7.SetTile(node.G + (float)Math.Sqrt(2), new Vector2(node.Position.x - 1, node.Position.y - 1), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node7))
                Neighbours.Add(node7);
        }

        Tile node8 = GridManager.Instance.GetTileAtPosition(new Vector2(node.Position.x - 1, node.Position.y + 1));

        if (node8 != null && (node8.Walkable || Unit.abilities.Contains(Abilities.Fly)) && (!CheckedNodes.Contains(node8) || node.G + (float)Math.Sqrt(2) < node8.G))
        {
            node8.SetTile(node.G + (float)Math.Sqrt(2), new Vector2(node.Position.x - 1, node.Position.y + 1), node.TargetPosition, node);
            if (!WaitingNodes.Contains(node8))
                Neighbours.Add(node8);
        }

        return Neighbours;
    }
}
