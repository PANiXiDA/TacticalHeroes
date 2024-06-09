using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public List<ScriptableUnit> _units;
    public BaseHero SelectedHero;

    public List<BaseUnit> PlayerUnits = new List<BaseUnit>();
    public List<BaseUnit> EnemyUnits = new List<BaseUnit>();
    public List<BaseUnit> allUnits = new List<BaseUnit>();

    public List<BaseUnit> ATB = new List<BaseUnit>();

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
        var units = _units.Where(u => u.Faction == Faction.Hero).Select(u => u.UnitPrefab).ToList();
        foreach (var unit in units)
        {
            var spawnedHero = Instantiate(unit);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero, randomSpawnTile);
        }
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }
    public void SpawnEnemies()
    {
        var units = _units.Where(u => u.Faction == Faction.Enemy).Select(u => u.UnitPrefab).ToList();

        foreach (var unit in units)
        {
            var spawnedEnemy = Instantiate(unit);

            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy, randomSpawnTile);
        }
        SetPlayerUnits();
        GameManager.Instance.ChangeState(GameState.SetATB);
    }
    public void SetATB()
    {
        //ATB.AddRange(PlayerUnits);
        //ATB.AddRange(EnemyUnits);
        //ATB.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));

        int count = 0;
        allUnits.AddRange(PlayerUnits);
        allUnits.AddRange(EnemyUnits);
        allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));
        while (count < 100)
        {
            var time = allUnits.FirstOrDefault().UnitTime;
            foreach (var unit in allUnits)
            {
                unit.UnitATB += unit.UnitInitiative * time;
                if (unit.UnitATB >= 100)
                {
                    unit.UnitATB -= 100;
                    ATB.Add(unit);
                    count++;
                }
                unit.UnitTime = (100 - unit.UnitATB) / unit.UnitInitiative;
            }
            allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));
        }
        MenuManager.Instance.ShowUnitsPortraits();
        if (ATB.First().Faction == Faction.Hero)
        {
            SetSelectedHero((BaseHero)ATB.First());
            Tile.Instance.SetHighlight(SelectedHero);
            GameManager.Instance.ChangeState(GameState.HeroesTurn);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
        }
    }
    public void UpdateATB(BaseUnit unit)
    {
        var unitTime = allUnits.FirstOrDefault().UnitTime;
        ATB.Remove(unit);
        foreach (var ATBunit in allUnits)
        {
            ATBunit.UnitATB += ATBunit.UnitInitiative * unitTime;
            if (ATBunit.UnitATB >= 100)
            {
                ATBunit.UnitATB -= 100;
                ATB.Add(ATBunit);
            }
            ATBunit.UnitTime = (100 - ATBunit.UnitATB) / ATBunit.UnitInitiative;
        }
        allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));
        MenuManager.Instance.ShowUnitsPortraits();
        if (EnemyUnits.Count == 0)
        {
            MenuManager.Instance.WinPanel();
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
        else if (PlayerUnits.Count == 0)
        {
            MenuManager.Instance.LosePanel();
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
        else if (ATB.First().Faction == Faction.Hero)
        {
            SetSelectedHero((BaseHero)ATB.First());
            Tile.Instance.SetHighlight(SelectedHero);
            GameManager.Instance.ChangeState(GameState.HeroesTurn);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
        }
    }

    private T GetRandomUnit<T>(Faction faction) where T: BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => UnityEngine.Random.value).First().UnitPrefab;
    }
    public void SetSelectedHero(BaseHero hero)
    {
        SelectedHero = hero;
    }
    public IEnumerator Attack(BaseUnit hero, BaseUnit enemy, bool meleeAttack, bool responseAttack)
    {
        if (meleeAttack)
        {
            hero.animator.Play("MeleeAttack");
        }
        else
        {
            hero.animator.Play("RangeAttack");
        }
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
        if (enemy.UnitHealth <= 0)
        {
            enemy.animator.Play("Death");
            enemy.OccupiedTile.OccupiedUnit = null;
            if (GameManager.Instance.GameState == GameState.HeroesTurn)
            {
                if (responseAttack == false)
                {
                    enemy.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    EnemyUnits.Remove(enemy);
                    allUnits.Remove(enemy);
                    ATB.RemoveAll(item => item == enemy);
                }
                else
                {
                    enemy.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    PlayerUnits.Remove(enemy);
                    allUnits.Remove(enemy);
                    ATB.RemoveAll(item => item == enemy);
                }
            }
            else
            {
                if (responseAttack == false)
                {
                    enemy.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    PlayerUnits.Remove(enemy);
                    allUnits.Remove(enemy);
                    ATB.RemoveAll(item => item == enemy);
                }
                else
                {
                    enemy.GetComponent<SpriteRenderer>().sortingOrder = -1;
                    EnemyUnits.Remove(enemy);
                    allUnits.Remove(enemy);
                    ATB.RemoveAll(item => item == enemy);
                }
            }
        }
        else if (enemy.UnitResponse == true && meleeAttack == true && responseAttack == false)
        {
            enemy.UnitResponse = false;
            yield return new WaitForSecondsRealtime(1);
            StartCoroutine(Attack(enemy, hero, true, true));
            if (GameManager.Instance.GameState == GameState.HeroesTurn && responseAttack == true)
            {
                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
    public Dictionary<Vector2, Tile> GetTilesForMove(BaseUnit unit)
    {
        Dictionary<Vector2, Tile> tilesForMove = new Dictionary<Vector2, Tile>();
        if (unit != null)
        {
            var grid = GridManager.Instance.GetGrid();
            foreach (var tile in grid)
            {
                PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(unit.OccupiedTile), tile.Key, unit);
                if (tile.Value.F <= unit.UnitSpeed && tile.Value.Walkable)
                {

                    tilesForMove.Add(tile.Key, tile.Value);
                }
            }
        }
        return tilesForMove;
    }
}
