using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Enumeration;
using System;

public class UnitManager : MonoBehaviour
{
    private System.Random random = new System.Random();

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
        if (GameManager.Instance.GameState == GameState.HeroesTurn)
        {
            SetSelectedHero(null);
        }

        BaseUnit currentUnit = allUnits.FirstOrDefault();
        currentUnit.GetComponent<SpriteRenderer>().sortingOrder = 1;
        var unitTime = currentUnit.UnitTime;
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
        unit.GetComponent<SpriteRenderer>().sortingOrder = 2;

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

        int damage = CalculateDamage(attacker, defender);

        defender.TakeMeleeDamage(attacker, defender);
        bool death = IsDeath(defender);

        if (death) defender.Death(responseAttack);

        if (!death && defender.UnitResponse && meleeAttack && !responseAttack)
        {
            await UniTask.Delay(1000);
            defender.UnitResponse = false;
            await Attack(defender, attacker, true, true);
        }
    }
    public bool IsDeath(BaseUnit unit)
    {
        if (unit.UnitHealth <= 0)
        {
            return true;
        }
        return false;
    }
    public virtual bool IsResponseAttack(BaseUnit unit)
    {
        if (GameManager.Instance.GameState == GameState.HeroesTurn)
        {
            if (unit.Faction == Faction.Hero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (unit.Faction == Faction.Enemy)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public int CalculateDamage(BaseUnit attacker, BaseUnit defender)
    {
        double baseDamage = UnityEngine.Random.Range(attacker.UnitMinDamage, attacker.UnitMaxDamage);
        double damageModifier = attacker.UnitAttack > defender.UnitDefence ?
            (1 + 0.05 * (attacker.UnitAttack - defender.UnitDefence)) :
            (1 / (1 + 0.05 * (defender.UnitDefence - attacker.UnitAttack)));

        int damage = (int)(baseDamage * damageModifier);

        MenuManager.Instance.ShowDamage(attacker, defender, damage);

        return damage;
    }

    public void RemoveUnit(BaseUnit unit, bool responseAttack)
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

    public virtual void ProbabilityLuckMorale(BaseUnit unit)
    {
        var probabilityLuck = Math.Pow(unit.UnitLuck / 10.0, 1 + unit.UnitSuccessfulLuck - unit.UnitFailedLuck * (unit.UnitLuck / 10.0 / (1 - unit.UnitLuck / 10.0)));
        var probabilityMorale = Math.Pow(unit.UnitMorale / 10.0, 1 + unit.UnitSuccessfulMorale - unit.UnitFailedMorale * (unit.UnitMorale / 10.0 / (1 - unit.UnitMorale / 10.0)));
    }

    public virtual void SetArcherParameters(BaseUnit unit)
    {
        if (unit.abilities.Contains(Abilities.Archer))
        {
            unit.UnitRange = 6;
            unit.UnitArrows = 10;
        }
    }
    public virtual void InitializeATB(BaseUnit unit)
    {
        unit.UnitATB = random.NextDouble() * (15 - 0) + 0;
        unit.UnitTime = (100 - unit.UnitATB) / unit.UnitInitiative;
    }
}