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

        public void SpawnHeroes()
        {
            var heroUnits = _units.Where(u => u.Faction == Faction.Hero).Select(u => u.UnitPrefab).ToList();

            foreach (var unit in heroUnits)
            {
                var spawnedHero = Instantiate(unit);
                var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

                UnitFactory.Instance.CreateOrUpdateUnitVisuals(spawnedHero);

                randomSpawnTile.SetUnit(spawnedHero, randomSpawnTile);
            }
            GameManager.Instance.ChangeState(GameState.SpawnEnemies);
        }

        public void SpawnEnemies()
        {
            var enemyUnits = _units.Where(u => u.Faction == Faction.Enemy).Select(u => u.UnitPrefab).ToList();

            foreach (var unit in enemyUnits)
            {
                var spawnedEnemy = Instantiate(unit);
                var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

                UnitFactory.Instance.CreateOrUpdateUnitVisuals(spawnedEnemy);

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
                    if (tile.Value.OccupiedUnit.Faction == Faction.Hero)
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

            if (GameManager.Instance.GameState == GameState.HeroesTurn)
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
            TurnManager.Instance.ATB.RemoveAll(item => item == unit);
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
