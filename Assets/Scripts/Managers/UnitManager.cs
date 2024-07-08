using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Enumeration;
using System;
using DG.Tweening.Core.Easing;
using UnityEditor.Experimental.GraphView;

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
    public void ChangeUnitFlip(BaseUnit attacker, Tile targetTile)
    {
        if (!IsUnitFlip(attacker))
        {
            if (attacker.Faction == Faction.Hero && attacker.OccupiedTile.Position.x > targetTile.Position.x
                || attacker.Faction == Faction.Enemy && attacker.OccupiedTile.Position.x < targetTile.Position.x)
            {
                var spriteRenderer = attacker.GetComponent<SpriteRenderer>();
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
    }
    public void SetOriginalUnitFlip(BaseUnit unit)
    {
        if (IsUnitFlip(unit))
        {
            var spriteRenderer = unit.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
    public bool IsUnitFlip(BaseUnit unit)
    {
        var spriteRenderer = unit.GetComponent<SpriteRenderer>();
        if (unit.Faction == Faction.Hero && spriteRenderer.flipX || unit.Faction == Faction.Enemy && !spriteRenderer.flipX)
        {
            return true;
        }
        return false;
    }
}