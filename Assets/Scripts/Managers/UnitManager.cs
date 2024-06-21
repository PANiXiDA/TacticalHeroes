using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public List<ScriptableUnit> _units;
    public BaseUnit SelectedHero;

    public List<BaseUnit> PlayerUnits = new List<BaseUnit>();
    public List<BaseUnit> EnemyUnits = new List<BaseUnit>();
    public List<BaseUnit> allUnits = new List<BaseUnit>();

    public List<BaseUnit> ATB = new List<BaseUnit>();

    void Awake()
    {
        Instance = this;
        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
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

    public void SpawnHeroes()
    {
        var heroUnits = _units.Where(u => u.Faction == Faction.Hero).Select(u => u.UnitPrefab).ToList();
        foreach (var unit in heroUnits)
        {
            var spawnedHero = Instantiate(unit);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();
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
            randomSpawnTile.SetUnit(spawnedEnemy, randomSpawnTile);
        }
        SetUnits();
        GameManager.Instance.ChangeState(GameState.SetATB);
    }

    public void SetATB()
    {
        allUnits.AddRange(PlayerUnits);
        allUnits.AddRange(EnemyUnits);
        allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));

        while (ATB.Count < 100)
        {
            var time = allUnits.FirstOrDefault().UnitTime;
            foreach (var unit in allUnits)
            {
                unit.UnitATB += unit.UnitInitiative * time;
                if (unit.UnitATB >= 100)
                {
                    unit.UnitATB -= 100;
                    ATB.Add(unit);
                }
                unit.UnitTime = (100 - unit.UnitATB) / unit.UnitInitiative;
            }
            allUnits.Sort((x, y) => x.UnitTime.CompareTo(y.UnitTime));
        }

        MenuManager.Instance.ShowUnitsPortraits();

        StartTurn(ATB.First());
    }

    public void UpdateATB()
    {
        var unitTime = allUnits.FirstOrDefault().UnitTime;
        ATB.RemoveAt(0);

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
        else
        {
            StartTurn(ATB.First());
        }
    }

    private void StartTurn(BaseUnit unit)
    {
        unit.UnitResponse = true;
        if (unit.Faction == Faction.Hero)
        {
            SetSelectedHero(unit);
            Tile.Instance.SetHighlight(SelectedHero);
            GameManager.Instance.ChangeState(GameState.HeroesTurn);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
        }
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => UnityEngine.Random.value).First().UnitPrefab;
    }

    public void SetSelectedHero(BaseUnit hero)
    {
        SelectedHero = hero;
    }

    public async UniTask Attack(BaseUnit attacker, BaseUnit defender, bool meleeAttack, bool responseAttack)
    {
        attacker.animator.Play(meleeAttack ? "MeleeAttack" : "RangeAttack");
        defender.animator.Play("TakeDamage");

        int damage = CalculateDamage(attacker, defender);
        MenuManager.Instance.ShowDamage(attacker, defender, damage);
        defender.UnitHealth -= damage;

        if (defender.UnitHealth <= 0)
        {
            defender.animator.Play("Death");
            defender.OccupiedTile.OccupiedUnit = null;

            RemoveUnit(defender, responseAttack);
        }
        else if (defender.UnitResponse && meleeAttack && !responseAttack)
        {
            defender.UnitResponse = false;
            await UniTask.Delay(1000);
            await Attack(defender, attacker, true, true);

            if (GameManager.Instance.GameState == GameState.HeroesTurn && responseAttack)
            {
                await UniTask.Delay(1000);
            }
        }
    }

    private int CalculateDamage(BaseUnit attacker, BaseUnit defender)
    {
        double baseDamage = UnityEngine.Random.Range(attacker.UnitMinDamage, attacker.UnitMaxDamage);
        double damageModifier = attacker.UnitAttack > defender.UnitDefence ?
            (1 + 0.05 * (attacker.UnitAttack - defender.UnitDefence)) :
            (1 / (1 + 0.05 * (defender.UnitDefence - attacker.UnitAttack)));

        return (int)(baseDamage * damageModifier);
    }

    private void RemoveUnit(BaseUnit unit, bool responseAttack)
    {
        unit.GetComponent<SpriteRenderer>().sortingOrder = -1;
        if (GameManager.Instance.GameState == GameState.HeroesTurn)
        {
            if (!responseAttack)
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
            if (!responseAttack)
            {
                PlayerUnits.Remove(unit);
            }
            else
            {
                EnemyUnits.Remove(unit);
            }
        }
        allUnits.Remove(unit);
        ATB.RemoveAll(item => item == unit);
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