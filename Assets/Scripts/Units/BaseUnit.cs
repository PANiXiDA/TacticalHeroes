using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using Assets.Scripts.Enumeration;
using Cysharp.Threading.Tasks;

public class BaseUnit : MonoBehaviour
{
    private System.Random random = new System.Random();

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
    public int? UnitRange;
    public int? UnitArrows;

    public int UnitMorale;
    public int UnitLuck;

    private int UnitFailedMorale;
    private int UnitSuccessfulMorale;
    private int UnitFailedLuck;
    private int UnitSuccessfulLuck;

    public List<Abilities> abilities;
    public Animator animator;

    public bool UnitResponse;
    public double UnitATB;
    public double UnitTime;

    public bool isBusy;

    protected virtual void Awake()
    {
        UnitFailedMorale = UnitSuccessfulMorale = UnitFailedLuck = UnitSuccessfulLuck = 0;
        SetSortingOrder();
        SetRange();
        InitializeATB();
    }

    protected virtual void SetSortingOrder()
    {
        if (abilities.Contains(Abilities.Fly))
        {
            GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else
        {
            GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    protected virtual void SetRange()
    {
        UnitRange = abilities.Contains(Abilities.Archer) ? 6 : null;
    }

    protected virtual void InitializeATB()
    {
        UnitMorale = 1;
        UnitATB = random.NextDouble() * (15 - 0) + 0;
        UnitTime = (100 - UnitATB) / UnitInitiative;
    }

    public virtual void ProbabilityLuckMorale()
    {
        var probabilityLuck = Math.Pow(UnitLuck / 10.0, 1 + UnitSuccessfulLuck - UnitFailedLuck * (UnitLuck / 10.0 / (1 - UnitLuck / 10.0)));
        var probabilityMorale = Math.Pow(UnitMorale / 10.0, 1 + UnitSuccessfulMorale - UnitFailedMorale * (UnitMorale / 10.0 / (1 - UnitMorale / 10.0)));
    }

    public virtual async UniTask Attack(Tile enemyTile, Tile tileForAttack)
    {
        isBusy = true;

        if (tileForAttack != null)
        {
            if (OccupiedTile != tileForAttack)
            {
                await Move(tileForAttack, false);
            }
            await MeleeAttack(enemyTile, true);
        }

        isBusy = false;
    }

    public virtual async UniTask Move(Tile tile, bool changeState)
    {
        isBusy = true;

        Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(this);
        if (tilesForMove.ContainsValue(tile))
        {
            Tile.Instance.DeleteHighlight();
            var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(OccupiedTile),
                GridManager.Instance.GetTileCoordinate(tile), this);
            path.Reverse();
            path.RemoveAt(0);

            Vector3[] path_ = path.Select(p => new Vector3(p.x, p.y, 0)).ToArray();

            animator.Play("Move");
            await transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);

            if (OccupiedTile != null)
            {
                OccupiedTile.OccupiedUnit = null;
            }

            tile.OccupiedUnit = this;
            OccupiedTile = tile;

            if (changeState)
            {
                UnitManager.Instance.SetSelectedHero(null);
                UnitManager.Instance.UpdateATB();
            }

            isBusy = false;
        }
    }

    public virtual async UniTask MeleeAttack(Tile enemyTile, bool changeState)
    {
        isBusy = true;

        var enemy = enemyTile.OccupiedUnit;
        var enemy_tile = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
        var hero_tile = GridManager.Instance.GetTileCoordinate(OccupiedTile);

        Tile.Instance.DeleteHighlight();
        if (Math.Abs(enemy_tile.x - hero_tile.x) <= 1 && Math.Abs(enemy_tile.y - hero_tile.y) <= 1)
        {
            await UnitManager.Instance.Attack(this, enemy, true, false);

            if (changeState)
            {
                UnitManager.Instance.SetSelectedHero(null);
                UnitManager.Instance.UpdateATB();
            }
        }

        isBusy = false;
    }

    public virtual async UniTask RangeAttack(Tile myTile, Tile enemyTile)
    {
        var enemyUnit = enemyTile.OccupiedUnit;
        var myUnit = myTile.OccupiedUnit;

        Tile.Instance.DeleteHighlight();
        await UnitManager.Instance.Attack(myUnit, enemyUnit, false, false);

        if (GameManager.Instance.GameState == GameState.HeroesTurn)
        {
            UnitManager.Instance.SetSelectedHero(null);
        }
        UnitManager.Instance.UpdateATB();
    }

    public virtual void TakeDamage(int damage)
    {
        UnitHealth -= damage;
        animator.Play("TakeDamage");
    }

    public virtual void Die()
    {
        animator.Play("Death");
        OccupiedTile.OccupiedUnit = null;
    }
}
