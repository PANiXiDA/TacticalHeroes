using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class BaseUnit : MonoBehaviour
{
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public int UnitAttack;
    public int UnitDefence;
    public int UnitHealth;
    public int UnitMinDamage;
    public int UnitMaxDamage;
    public int UnitInitiative;
    public int UnitSpeed;
    public Animator animator;

    public IEnumerator Attack(Tile tile, Tile tilesForAttack)
    {
        float startTime = Time.realtimeSinceStartup;

        if (tilesForAttack != null)
        {
            if (UnitManager.Instance.SelectedHero.OccupiedTile != tilesForAttack)
            {
                Move(tilesForAttack, false);
                yield return new WaitForSecondsRealtime(1);
            }
            Attack(tile, true, false);
        }
    }
    public void Move(Tile tile, bool changeState)
    {
        Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);
        if (tilesForMove.ContainsValue(tile))
        {
            //Tile.Instance.SetHighlight(UnitManager.Instance.SelectedHero); // Убираем подсветку после перемещения юнита
            Tile.Instance.DeleteHighlight(); // Убираем подсветку после перемещения юнита

            var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(UnitManager.Instance.SelectedHero.OccupiedTile), GridManager.Instance.GetTileCoordinate(tile));
            path.Reverse();
            path.RemoveAt(0);
            Vector3[] path_ = new Vector3[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                path_[i] = new Vector3(path[i].x, path[i].y, 0);
            }
            UnitManager.Instance.SelectedHero.animator.Play("Move");
            UnitManager.Instance.SelectedHero.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear);

            if (UnitManager.Instance.SelectedHero.OccupiedTile != null)
            {
                UnitManager.Instance.SelectedHero.OccupiedTile.OccupiedUnit = null;
            }
            tile.OccupiedUnit = UnitManager.Instance.SelectedHero;
            UnitManager.Instance.SelectedHero.OccupiedTile = tile;
            if (changeState)
            {
                UnitManager.Instance.SetSelectedHero(null);
                GameManager.Instance.ChangeState(GameState.EnemiesTurn);
            }
        }
    }
    public void Attack(Tile tile, bool changeState, bool changeHighlight)
    {
        var enemy = (BaseEnemy)tile.OccupiedUnit;
        var enemy_tile = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
        var hero_tile = GridManager.Instance.GetTileCoordinate(UnitManager.Instance.SelectedHero.OccupiedTile);
        var hero = GridManager.Instance.GetTileAtPosition(hero_tile).OccupiedUnit;
        Tile.Instance.DeleteHighlight();
        if (Math.Abs(enemy_tile.x - hero_tile.x) <= 1 && Math.Abs(enemy_tile.y - hero_tile.y) <= 1)
        {
            var health = UnitManager.Instance.Attack(UnitManager.Instance.SelectedHero, enemy);
            if (health <= 0)
            {
                //if (changeHighlight)
                //    Tile.Instance.SetHighlight(UnitManager.Instance.SelectedHero);// Убираем подсветку после атаки вражеского юнита
                enemy.animator.Play("Death");
                enemy.OccupiedTile.OccupiedUnit = null;
                UnitManager.Instance.EnemyUnits.Remove(enemy);
                //Destroy(enemy.gameObject);
                if (changeState)
                {
                    UnitManager.Instance.SetSelectedHero(null);
                    GameManager.Instance.ChangeState(GameState.EnemiesTurn);
                }
            }
            else
            {
                //if (changeHighlight)
                //    Tile.Instance.SetHighlight(UnitManager.Instance.SelectedHero);// Убираем подсветку после атаки вражеского юнита
                if (changeState)
                {
                    UnitManager.Instance.SetSelectedHero(null);
                    GameManager.Instance.ChangeState(GameState.EnemiesTurn);
                }
            }
        }
    }
}
