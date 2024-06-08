using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using Assets.Scripts.Enumeration;

public class BaseUnit : MonoBehaviour
{
    System.Random random = new System.Random();

    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int UnitAttack;
    public int UnitDefence;
    public int UnitHealth;
    public int UnitMinDamage;
    public int UnitMaxDamage;
    public double UnitInitiative;
    public int UnitSpeed;
    public int UnitRange;

    public List<Abilities> abilities;
    public Animator animator;

    public bool UnitResponse;
    public double UnitATB;
    public double UnitTime;

    void Awake()
    {
        UnitATB = random.NextDouble() * (15 - 0) + 0;
        UnitTime = (100-UnitATB)/UnitInitiative;
    }

    public virtual IEnumerator Attack(Tile enemyTile, Tile tileForAttack)
    {
        UnitManager.Instance.SelectedHero.UnitResponse = true;
        if (tileForAttack != null)
        {
            if (UnitManager.Instance.SelectedHero.OccupiedTile != tileForAttack)
            {
                Move(tileForAttack, false);
                yield return new WaitForSecondsRealtime(1);
            }
            MeleeAttack(enemyTile, true, false);
        }
    }
    public virtual void Move(Tile tile, bool changeState)
    {
        UnitManager.Instance.SelectedHero.UnitResponse = true;
        Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);
        if (tilesForMove.ContainsValue(tile))
        {
            Tile.Instance.DeleteHighlight();
            var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(UnitManager.Instance.SelectedHero.OccupiedTile), 
                GridManager.Instance.GetTileCoordinate(tile), UnitManager.Instance.SelectedHero);
            path.Reverse();
            path.RemoveAt(0);
            Vector3[] path_ = new Vector3[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                path_[i] = new Vector3(path[i].x, path[i].y, 0);
            }
            UnitManager.Instance.SelectedHero.animator.Play("Move");
            UnitManager.Instance.SelectedHero.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);
            if (UnitManager.Instance.SelectedHero.OccupiedTile != null)
            {
                UnitManager.Instance.SelectedHero.OccupiedTile.OccupiedUnit = null;
            }
            tile.OccupiedUnit = UnitManager.Instance.SelectedHero;
            UnitManager.Instance.SelectedHero.OccupiedTile = tile;
            if (changeState)
            {
                var hero = UnitManager.Instance.SelectedHero;
                UnitManager.Instance.SetSelectedHero(null);
                UnitManager.Instance.UpdateATB(hero);
            }
        }
    }
    public virtual void MeleeAttack(Tile enemyTile, bool changeState, bool changeHighlight)
    {
        var enemy = enemyTile.OccupiedUnit;
        var enemy_tile = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
        var hero_tile = GridManager.Instance.GetTileCoordinate(UnitManager.Instance.SelectedHero.OccupiedTile);
        var hero = GridManager.Instance.GetTileAtPosition(hero_tile).OccupiedUnit;
        Tile.Instance.DeleteHighlight();
        if (Math.Abs(enemy_tile.x - hero_tile.x) <= 1 && Math.Abs(enemy_tile.y - hero_tile.y) <= 1)
        {
            StartCoroutine(UnitManager.Instance.Attack(UnitManager.Instance.SelectedHero, enemy, true, false));
            if (changeState)
            {
                UnitManager.Instance.SetSelectedHero(null);
                UnitManager.Instance.UpdateATB(hero);
            }
        }
    }
    public virtual void RangeAttack(Tile myTile, Tile enemyTile)
    {
        var enemyUnit = enemyTile.OccupiedUnit;
        var myUnit = myTile.OccupiedUnit;
        Tile.Instance.DeleteHighlight();
        StartCoroutine(UnitManager.Instance.Attack(myUnit, enemyUnit, false, false));
        if (GameManager.Instance.GameState == GameState.HeroesTurn)
        {
            UnitManager.Instance.SetSelectedHero(null);
            UnitManager.Instance.UpdateATB(myUnit);
        }
        else
        {
            UnitManager.Instance.UpdateATB(myUnit);
        }
    }

    public static explicit operator GameObject(BaseUnit v)
    {
        throw new NotImplementedException();
    }
}
