using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public List<ScriptableUnit> _units;
    public BaseHero SelectedHero;

    public List<BaseUnit> PlayerUnits = new List<BaseUnit>();
    public List<BaseUnit> EnemyUnits = new List<BaseUnit>();

    void Awake()
    {
        Instance = this;

        //из наших ресурсов выгружает всех юнитов в список
        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }
    public void SetPlayerUnits()
    {
        var grid = GridManager.Instance.GetGrid();
        foreach (var tile in grid)
        {
            if (tile.Value.OccupiedUnit is not null)
            {
                if (tile.Value.OccupiedUnit.Faction == Faction.Hero)
                    PlayerUnits.Add(tile.Value.OccupiedUnit);
                else
                    EnemyUnits.Add(tile.Value.OccupiedUnit);
            }
        }
    }
    public void SpawnHeroes()
    {
        var heroCount = 1;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero, randomSpawnTile);
        }
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }
    public void SpawnEnemies()
    {
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy, randomSpawnTile);
        }
        SetPlayerUnits();
        GameManager.Instance.ChangeState(GameState.HeroesTurn);
    }

    private T GetRandomUnit<T>(Faction faction) where T: BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => UnityEngine.Random.value).First().UnitPrefab;
    }
    public void SetSelectedHero(BaseHero hero)
    {
        SelectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }
    public int Attack(BaseUnit hero, BaseUnit enemy)
    {
        hero.animator.Play("Attack");
        enemy.animator.Play("TakeDamage");
        double d;
        if (hero.UnitAttack > enemy.UnitDefence)
        {
            d = UnityEngine.Random.Range(hero.UnitMinDamage, hero.UnitMaxDamage) * (1 + 0.05 * (hero.UnitAttack - enemy.UnitDefence));
        }
        else if(hero.UnitAttack < enemy.UnitDefence)
        {
            d = UnityEngine.Random.Range(hero.UnitMinDamage, hero.UnitMaxDamage) / (1 + 0.05 * (enemy.UnitDefence - hero.UnitAttack));
        }
        else
        {
            d = UnityEngine.Random.Range(hero.UnitMinDamage, hero.UnitMaxDamage);
        }
        int damage = (int)d;
        MenuManager.Instance.ShowDamage(hero, enemy, damage);
        enemy.UnitHealth = enemy.UnitHealth - damage;
        return enemy.UnitHealth;
    }
    public Dictionary<Vector2, Tile> GetTilesForMove(BaseUnit unit)
    {
        Dictionary<Vector2, Tile> tilesForMove = new Dictionary<Vector2, Tile>();
        if (unit != null)
        {
            var grid = GridManager.Instance.GetGrid();
            foreach (var tile in grid)
            {
                PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(unit.OccupiedTile), tile.Key);
                if (tile.Value.F <= unit.UnitSpeed && tile.Value.Walkable)
                {

                    tilesForMove.Add(tile.Key, tile.Value);
                }
            }
        }
        return tilesForMove;
    }
}
