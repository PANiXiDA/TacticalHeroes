using Assets.Scripts.Enumerations;
using Assets.Scripts.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        public List<ScriptableUnit> _units;


        public List<BaseUnit> PlayerUnits = new List<BaseUnit>();
        public List<BaseUnit> EnemyUnits = new List<BaseUnit>();

        void Awake()
        {
            Instance = this;
            _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        }

        public void SetUnitsSettings()
        {
            foreach (var unit in  _units)
            {
                if (unit.UnitPrefab.Faction == GameManager.Instance.PlayerFaction)
                {
                    unit.UnitPrefab.Side = Side.Player;
                }
                else
                {
                    unit.UnitPrefab.Side = Side.Enemy;
                }
            }
            GameManager.Instance.ChangeState(GameState.SpawnPlayerUnits);
        }

        public void SpawnPlayerUnits()
        {
            var heroUnits = _units.Where(u => u.UnitPrefab.Side == Side.Player && u.IsActive).Select(u => u.UnitPrefab).ToList();

            foreach (var unit in heroUnits)
            {
                var spawnedHero = Instantiate(unit);
                var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

                UnitFactory.Instance.CreateOrUpdateUnitVisuals(spawnedHero);

                randomSpawnTile.SetUnit(spawnedHero, randomSpawnTile);
            }
            GameManager.Instance.ChangeState(GameState.SpawnEnemyUnits);
        }

        public void SpawnEnemyUnits()
        {
            var enemyUnits = _units.Where(u => u.UnitPrefab.Side == Side.Enemy && u.IsActive).Select(u => u.UnitPrefab).ToList();

            foreach (var unit in enemyUnits)
            {
                var spawnedEnemy = Instantiate(unit);
                var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

                if (GameManager.Instance.GameDifficulty == DifficultyLevel.Middle)
                {
                    spawnedEnemy.UnitCount *= 2;
                }
                else if (GameManager.Instance.GameDifficulty == DifficultyLevel.Hard)
                {
                    spawnedEnemy.UnitCount *= 3;
                }

                UnitFactory.Instance.CreateOrUpdateUnitVisuals(spawnedEnemy);

                var spriteRenderer = spawnedEnemy.GetComponent<SpriteRenderer>();
                spriteRenderer.flipX = !spriteRenderer.flipX;

                randomSpawnTile.SetUnit(spawnedEnemy, randomSpawnTile);
            }
            SetUnits();
            GameManager.Instance.ChangeState(GameState.SetATB);
        }

        public void SetUnits()
        {
            var grid = GridManager.Instance.GetGrid();
            foreach (var tile in grid)
            {
                if (tile.Value.OccupiedUnit != null)
                {
                    if (tile.Value.OccupiedUnit.Side == Side.Player)
                    {
                        PlayerUnits.Add(tile.Value.OccupiedUnit);
                    }
                    else
                    {
                        EnemyUnits.Add(tile.Value.OccupiedUnit);
                    }
                }
            }
        }

        public void RemoveUnit(BaseUnit unit)
        {
            var responseAttack = UnitManager.Instance.IsResponseAttack(unit);

            unit.GetComponent<SpriteRenderer>().sortingOrder = -1;

            if (GameManager.Instance.GameState == GameState.PlayerTurn)
            {
                if (responseAttack)
                {
                    EnemyUnits.Remove(unit);
                }
                else
                {
                    PlayerUnits.Remove(unit);
                }
            }
            else
            {
                if (responseAttack)
                {
                    PlayerUnits.Remove(unit);
                }
                else
                {
                    EnemyUnits.Remove(unit);
                }
            }
            TurnManager.Instance.allUnits.Remove(unit);
            TurnManager.Instance.ATB.RemoveAll(item => item.Value == unit);
        }
        public void DestroyUnitChildrenObjects(BaseUnit unit)
        {
            foreach (Transform child in unit.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
