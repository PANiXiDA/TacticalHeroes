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
            //SetUnitsForNeutralFaction();
            GameManager.Instance.ChangeState(GameState.SpawnPlayerUnits);
        }

        public void SetUnitsForNeutralFaction()
        {

            var activeUnitsByTier = new Dictionary<Tier, ScriptableUnit>();

            var neutralUnitsByTier = _units
                .Where(u => u.UnitPrefab.Faction == Faction.Neutral)
                .GroupBy(u => u.UnitPrefab.Tier)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var tier in neutralUnitsByTier.Keys)
            {
                var unitsOfTier = neutralUnitsByTier[tier];
                if (unitsOfTier.Any())
                {
                    var randomUnit = unitsOfTier[Random.Range(0, unitsOfTier.Count)];
                    activeUnitsByTier[tier] = randomUnit;
                }
            }

            foreach (var unit in _units.Where(u => u.UnitPrefab.Faction == Faction.Neutral))
            {
                unit.IsActive = false;
            }

            foreach (var unit in activeUnitsByTier.Values)
            {
                unit.IsActive = true;
            }
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
            if (GameManager.Instance.GameDifficulty != DifficultyLevel.Hard)
            {
                GameManager.Instance.ChangeState(GameState.SpawnEnemyUnits);
            }
            else
            {
                if (GameManager.Instance.PlayerFaction == Faction.Neutral)
                {

                }
                else if (GameManager.Instance.PlayerFaction == Faction.Citadel)
                {
                    SpawnRory();
                }
            }
        }

        public void SpawnRory()
        {
            var rory = _units.FirstOrDefault(u => u.UnitPrefab.Faction == Faction.Rory);
            var spawnedRory = Instantiate(rory.UnitPrefab);

            var rorySpawnTile = GridManager.Instance.GetRorySpawnTile();

            UnitFactory.Instance.CreateOrUpdateUnitVisuals(spawnedRory);

            var spriteRenderer = spawnedRory.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = !spriteRenderer.flipX;

            rorySpawnTile.SetUnit(spawnedRory, rorySpawnTile);
            SetUnits();
            GameManager.Instance.ChangeState(GameState.SetATB);
        }

        public void SpawnEnemyUnits()
        {
            Faction playerFaction = GameManager.Instance.PlayerFaction.Value;
            Faction enemyFaction = playerFaction == Faction.Citadel ? Faction.Neutral : Faction.Citadel;

            var enemyUnits = _units.Where(u => u.UnitPrefab.Side == Side.Enemy && u.IsActive && u.UnitPrefab.Faction == enemyFaction).Select(u => u.UnitPrefab).ToList();

            foreach (var unit in enemyUnits)
            {
                var spawnedEnemy = Instantiate(unit);
                var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

                if (GameManager.Instance.GameDifficulty == DifficultyLevel.Medium)
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
