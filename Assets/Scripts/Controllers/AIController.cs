using Assets.Scripts.Managers;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class AIController : MonoBehaviour
    {
        public static AIController Instance;
        private void Awake()
        {
            Instance = this;
        }

        public async UniTask ExecuteAITurn()
        {
            await UniTask.Delay(1000);

            var enemy = TurnManager.Instance.ATB.First().Value;
            var player = SelectRandomPlayer(SpawnManager.Instance.PlayerUnits);

            if (UnitManager.Instance.IsRangeAttackPossible(enemy))
            {
                await enemy.RangeAttack(enemy, player);
            }
            else
            {
                var enemyCoordinates = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
                var playerCoordinates = GridManager.Instance.GetTileCoordinate(player.OccupiedTile);

                var enemyTilesForMove = UnitManager.Instance.GetTilesForMove(enemy);
                var playerTilesForMove = UnitManager.Instance.GetTilesForMove(player);

                var pathToEnemy = GetPathToEnemy(enemyCoordinates, playerCoordinates, enemy);

                // Разворачиваем, чтобы путь шел от enemy к player и удаляем enemy из пути
                if (pathToEnemy.Count > 0)
                {
                    pathToEnemy.Reverse();
                    pathToEnemy.RemoveAt(0);
                }
                else
                {
                    Debug.Log("Путь меньше нуля");
                    Debug.Log(pathToEnemy);
                }

                if (enemyTilesForMove.ContainsKey(pathToEnemy[pathToEnemy.Count-1]))
                {
                    var targetTile = GridManager.Instance.GetTileAtPosition(pathToEnemy[pathToEnemy.Count-1]);
                    await enemy.MeleeAttack(enemy, player, targetTile);
                }
                else
                {
                    // Разворачиваем, чтобы быть шел от player к enemy, тк в Move нужно передать самую удаленную клетку от Enemy
                    pathToEnemy.Reverse();

                    enemyTilesForMove.Except(playerTilesForMove);

                    foreach (var tileCoordinates in pathToEnemy)
                    {
                        if (enemyTilesForMove.ContainsKey(tileCoordinates))
                        {
                            var targetTile = GridManager.Instance.GetTileAtPosition(tileCoordinates);
                            await enemy.Move(enemy, targetTile);
                            break;
                        }
                    }
                }
            }
        }

        private BaseUnit SelectRandomPlayer(List<BaseUnit> playerUnits)
        {
            var randomNumber = UnityEngine.Random.Range(0, playerUnits.Count);
            return playerUnits[randomNumber];
        }

        private List<Vector2> GetPathToEnemy(Vector2 enemyCoordinates, Vector2 playerCoordinates, BaseUnit enemy)
        {
            List<List<Vector2>> paths = new List<List<Vector2>>();

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
                var targetTilePosition = new Vector2(playerCoordinates.x + direction.x, playerCoordinates.y + direction.y);
                var targetTile = GridManager.Instance.GetTileAtPosition(targetTilePosition);

                if (targetTile != null && targetTile.Walkable)
                {
                    paths.Add(PathFinder.Instance.GetPath(enemyCoordinates, targetTilePosition, enemy));
                }
            }

            paths.OrderBy(path => path.Count);

            return paths.FirstOrDefault();
        }
        private List<Vector2> GetTilesAroundPlayer(Vector2 playerCoordinates)
        {
            List<Vector2> tiles = new List<Vector2>();

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
                tiles.Add(new Vector2(playerCoordinates.x + direction.x, playerCoordinates.y + direction.y));
            }

            return tiles;
        }
    }
}
