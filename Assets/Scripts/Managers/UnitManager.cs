using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Enumeration;
using System;
using DG.Tweening.Core.Easing;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public BaseUnit SelectedHero;

    void Awake()
    {
        Instance = this;
    }

    public void SetSelectedHero(BaseUnit hero)
    {
        SelectedHero = hero;
    }

    public bool IsDead(BaseUnit unit)
    {
        if (unit.UnitHealth <= 0)
        {
            return true;
        }
        return false;
    }
    public virtual bool IsResponseAttack(BaseUnit unit)
    {
        return (GameManager.Instance.GameState == GameState.HeroesTurn && unit.Faction != Faction.Hero)
            || (GameManager.Instance.GameState != GameState.HeroesTurn && unit.Faction != Faction.Enemy);
    }

    public Dictionary<Vector2, Tile> GetTilesForMove(BaseUnit unit)
    {
        Dictionary<Vector2, Tile> tilesForMove = new Dictionary<Vector2, Tile>();

        if (unit != null)
        {
            var grid = GridManager.Instance.GetGrid();
            foreach (var tile in grid)
            {
                PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(unit.OccupiedTile), tile.Key, unit);

                if (tile.Value.F <= unit.UnitSpeed && tile.Value.Walkable)
                {
                    tilesForMove.Add(tile.Key, tile.Value);
                }
            }
        }

        return tilesForMove;
    }
}