using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using Assets.Scripts.Enumeration;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Assets.Scripts.Enumerations;

public class ArtificialIntelligence : MonoBehaviour
{
    public static ArtificialIntelligence Instance;
    private void Awake()
    {
        Instance = this;
    }
    public async UniTask Waiter()
    {
        await UniTask.Delay(1000);
        List<BaseUnit> PlayerUnits = UnitManager.Instance.PlayerUnits;
        List<BaseUnit> EnemyUnits = UnitManager.Instance.EnemyUnits;
        int randomNumberEnemy = 0;
        int randomNumberPlayer = 0;
        var enemy = UnitManager.Instance.ATB.First();
        if (EnemyUnits.Count > 0 && PlayerUnits.Count > 0)
        {
            System.Random random = new System.Random();
            randomNumberEnemy = random.Next(0, EnemyUnits.Count());
            randomNumberPlayer = random.Next(0, PlayerUnits.Count());
            var player = PlayerUnits[randomNumberPlayer];
            if (!enemy.abilities.Contains(Abilities.Archer))
            {
                AIaction(randomNumberEnemy, randomNumberPlayer);
            }
            else
            {
                await enemy.RangeAttack(enemy, player);
                if (EnemyUnits.Count == 0 || PlayerUnits.Count == 0)
                {
                    await UniTask.Delay(2000);
                }
                return;
            }
            await UniTask.Delay(1000);
        }
        if (EnemyUnits.Count > 0 && PlayerUnits.Count > 0)
        {
            var player = PlayerUnits[randomNumberPlayer];
            var enemy_tile = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
            var player_tile = GridManager.Instance.GetTileCoordinate(player.OccupiedTile);
            if (Mathf.Abs(enemy_tile.x - player_tile.x) <= 1 && Mathf.Abs(enemy_tile.y - player_tile.y) <= 1)
            {
                await UnitManager.Instance.Attack(enemy, player, true, false);
                await UniTask.Delay(1000);
                if (enemy.abilities.Contains(Abilities.Archer) && EnemyUnits.Contains(enemy))
                {
                    await UnitManager.Instance.Attack(enemy, player, true, false);
                }
            }
        }
        if (EnemyUnits.Count == 0 || PlayerUnits.Count == 0)
        {
            await UniTask.Delay(2000);
        }
        UnitManager.Instance.UpdateATB();
    }
    public async void AIaction(int randomNumberEnemy, int randomNumberPlayer)
    {
        if (GameManager.Instance.GameState != GameState.EnemiesTurn) return;

        List<BaseUnit> PlayerUnits = UnitManager.Instance.PlayerUnits;
        List<BaseUnit> EnemyUnits = UnitManager.Instance.EnemyUnits;

        if (EnemyUnits.Count > 0)
        {
            var enemy = UnitManager.Instance.ATB.First();
            var player = PlayerUnits[randomNumberPlayer];
            player.OccupiedTile.OccupiedUnit = null;
            var enemy_coord = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
            var player_coord = GridManager.Instance.GetTileCoordinate(player.OccupiedTile);

            var EnemyTilesForMove = UnitManager.Instance.GetTilesForMove(enemy);
            var PlayerTilesForMove = UnitManager.Instance.GetTilesForMove(player);

            if (EnemyTilesForMove.ContainsKey(player_coord))
            {
                var PathToEnemy = PathFinder.Instance.GetPath(enemy_coord, player_coord, enemy);
                player.OccupiedTile.OccupiedUnit = player;
                foreach (var tile in PathToEnemy)
                {
                    if (EnemyTilesForMove.ContainsKey(tile) && player.OccupiedTile != GridManager.Instance.GetTileAtPosition(tile))
                    {
                        var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile), tile, enemy);
                        path.Reverse();
                        path.RemoveAt(0);
                        Vector3[] path_ = new Vector3[path.Count];
                        for (int i = 0; i < path.Count; i++)
                        {
                            path_[i] = new Vector3(path[i].x, path[i].y, 0);
                        }
                        enemy.animator.Play("Move");
                        await enemy.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D)
                            .SetEase(Ease.Linear);

                        if (enemy.OccupiedTile != null)
                        {
                            enemy.OccupiedTile.OccupiedUnit = null;
                        }
                        GridManager.Instance.GetTileAtPosition(tile).OccupiedUnit = enemy;
                        enemy.OccupiedTile = GridManager.Instance.GetTileAtPosition(tile);
                        break;
                    }
                }
            }
            else
            {
                var PathToEnemy = PathFinder.Instance.GetPath(enemy_coord, player_coord, player);
                player.OccupiedTile.OccupiedUnit = player;
                EnemyTilesForMove.Except(PlayerTilesForMove);
                foreach (var tile in PathToEnemy)
                {
                    if (EnemyTilesForMove.ContainsKey(tile) && player.OccupiedTile != GridManager.Instance.GetTileAtPosition(tile))
                    {
                        var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile), tile, enemy);
                        path.Reverse();
                        path.RemoveAt(0);
                        Vector3[] path_ = new Vector3[path.Count];
                        for (int i = 0; i < path.Count; i++)
                        {
                            path_[i] = new Vector3(path[i].x, path[i].y, 0);
                        }
                        enemy.animator.Play("Move");
                        await enemy.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D)
                            .SetEase(Ease.Linear);

                        if (enemy.OccupiedTile != null)
                        {
                            enemy.OccupiedTile.OccupiedUnit = null;
                        }
                        GridManager.Instance.GetTileAtPosition(tile).OccupiedUnit = enemy;
                        enemy.OccupiedTile = GridManager.Instance.GetTileAtPosition(tile);
                        break;
                    }
                }
            }
        }
    }
}
