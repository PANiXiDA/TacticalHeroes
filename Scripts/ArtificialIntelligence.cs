using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ArtificialIntelligence : MonoBehaviour
{
    public static ArtificialIntelligence Instance;
    private void Awake()
    {
        Instance = this;
    }
    public IEnumerator Waiter()
    {
        yield return new WaitForSecondsRealtime((float)1.5);
        AIaction();
        yield return new WaitForSecondsRealtime(1);
        List<BaseUnit> PlayerUnits = UnitManager.Instance.PlayerUnits;
        List<BaseUnit> EnemyUnits = UnitManager.Instance.EnemyUnits;
        if (EnemyUnits.Count > 0 && PlayerUnits.Count > 0)
        {
            var enemy = EnemyUnits[0];
            var player = PlayerUnits[0];
            var enemy_tile = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
            var player_tile = GridManager.Instance.GetTileCoordinate(player.OccupiedTile);
            if (Mathf.Abs(enemy_tile.x - player_tile.x) <= 1 && Mathf.Abs(enemy_tile.y - player_tile.y) <= 1)
            {
                var health = UnitManager.Instance.Attack(enemy, player);
                if (health <= 0)
                {
                    player.animator.Play("Death");
                    player.OccupiedTile.OccupiedUnit = null;
                    UnitManager.Instance.PlayerUnits.Remove(player);
                    //Destroy(enemy.gameObject);
                }
            }
        }
        if (EnemyUnits.Count == 0 || PlayerUnits.Count == 0)
        {
            yield return new WaitForSecondsRealtime(2);
            SceneManager.LoadScene(0);
        }
        yield return new WaitForSecondsRealtime((float)1.5);
    }
    public void AIaction()
    {
        if (GameManager.Instance.GameState != GameState.EnemiesTurn) return;

        List<BaseUnit> PlayerUnits = UnitManager.Instance.PlayerUnits;
        List<BaseUnit> EnemyUnits = UnitManager.Instance.EnemyUnits;

        if (EnemyUnits.Count > 0)
        {
            var enemy = EnemyUnits[0];
            var player = PlayerUnits[0];
            player.OccupiedTile.OccupiedUnit = null;
            var enemy_coord = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
            var player_coord = GridManager.Instance.GetTileCoordinate(player.OccupiedTile);

            var EnemyTilesForMove = UnitManager.Instance.GetTilesForMove(enemy);
            var PlayerTilesForMove = UnitManager.Instance.GetTilesForMove(player);

            if (EnemyTilesForMove.ContainsKey(player_coord))
            {
                var PathToEnemy = PathFinder.Instance.GetPath(enemy_coord, player_coord);
                player.OccupiedTile.OccupiedUnit = player;
                foreach (var tile in PathToEnemy)
                {
                    if (EnemyTilesForMove.ContainsKey(tile) && player.OccupiedTile != GridManager.Instance.GetTileAtPosition(tile))
                    {
                        var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile), tile);
                        path.Reverse();
                        path.RemoveAt(0);
                        Vector3[] path_ = new Vector3[path.Count];
                        for (int i = 0; i < path.Count; i++)
                        {
                            path_[i] = new Vector3(path[i].x, path[i].y, 0);
                        }
                        enemy.animator.Play("Move");
                        enemy.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D)
                            .SetEase(Ease.Linear);

                        if (enemy.OccupiedTile != null)
                        {
                            enemy.OccupiedTile.OccupiedUnit = null;
                        }
                        GridManager.Instance.GetTileAtPosition(tile).OccupiedUnit = enemy;
                        enemy.OccupiedTile = GridManager.Instance.GetTileAtPosition(tile);
                        //Tile.Instance.SetUnit(enemy, GridManager.Instance.GetTileAtPosition(tile));
                        break;
                    }
                }
            }
            else
            {
                var PathToEnemy = PathFinder.Instance.GetPath(enemy_coord, player_coord);
                player.OccupiedTile.OccupiedUnit = player;
                EnemyTilesForMove.Except(PlayerTilesForMove);
                foreach (var tile in PathToEnemy)
                {
                    if (EnemyTilesForMove.ContainsKey(tile) && player.OccupiedTile != GridManager.Instance.GetTileAtPosition(tile))
                    {
                        var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile), tile);
                        path.Reverse();
                        path.RemoveAt(0);
                        Vector3[] path_ = new Vector3[path.Count];
                        for (int i = 0; i < path.Count; i++)
                        {
                            path_[i] = new Vector3(path[i].x, path[i].y, 0);
                        }
                        enemy.animator.Play("Move");
                        enemy.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D)
                            .SetEase(Ease.Linear);

                        if (enemy.OccupiedTile != null)
                        {
                            enemy.OccupiedTile.OccupiedUnit = null;
                        }
                        GridManager.Instance.GetTileAtPosition(tile).OccupiedUnit = enemy;
                        enemy.OccupiedTile = GridManager.Instance.GetTileAtPosition(tile);
                        //Tile.Instance.SetUnit(enemy, GridManager.Instance.GetTileAtPosition(tile));
                        break;
                    }
                }
            }
        }
        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }
}
