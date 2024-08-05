using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Managers;
using System.Linq;
using Unity.Mathematics;
using Assets.Scripts.UI;
using Assets.Scripts.IActions;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public BaseUnit SelectedHero;

    void Awake()
    {
        Instance = this;
    }

    public void SetSelectedHero(BaseUnit hero)
    {
        SelectedHero = hero;
    }

    public bool IsDead(BaseUnit unit)
    {
        if (unit.UnitCount <= 0)
        {
            return true;
        }
        return false;
    }
    public virtual bool IsResponseAttack(BaseUnit unit)
    {
        return (GameManager.Instance.GameState == GameState.PlayerTurn && unit.Side != Side.Player)
            || (GameManager.Instance.GameState != GameState.PlayerTurn && unit.Side != Side.Enemy);
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
    public void ChangeUnitFlip(BaseUnit attacker, Tile targetTile)
    {
        if (!IsUnitFlip(attacker))
        {
            if (attacker.Side == Side.Player && GridManager.Instance.GetTileCoordinate(attacker.OccupiedTile).x > GridManager.Instance.GetTileCoordinate(targetTile).x
                || attacker.Side == Side.Enemy && GridManager.Instance.GetTileCoordinate(attacker.OccupiedTile).x < GridManager.Instance.GetTileCoordinate(targetTile).x)
            {
                var spriteRenderer = attacker.GetComponent<SpriteRenderer>();
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
    }
    public void SetOriginalUnitFlip(BaseUnit unit)
    {
        if (IsUnitFlip(unit))
        {
            var spriteRenderer = unit.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
    public bool IsUnitFlip(BaseUnit unit)
    {
        var spriteRenderer = unit.GetComponent<SpriteRenderer>();
        if (unit.Side == Side.Player && spriteRenderer.flipX || unit.Side == Side.Enemy && !spriteRenderer.flipX)
        {
            return true;
        }
        return false;
    }
    public void PlayAttackAnimation(BaseUnit attacker, BaseUnit defender)
    {
        if (attacker.OccupiedTile.Position.y > defender.OccupiedTile.Position.y)
        {
            attacker.animator.Play("BottomMeleeAttack");
        }
        else if (attacker.OccupiedTile.Position.y == defender.OccupiedTile.Position.y)
        {
            attacker.animator.Play("FrontMeleeAttack");
        }
        else if (attacker.OccupiedTile.Position.y < defender.OccupiedTile.Position.y)
        {
            attacker.animator.Play("TopMeleeAttack");
        }
    }
    public bool IsEnemyAround(BaseUnit unit)
    {
        List<Vector2Int> directions = new List<Vector2Int>()
        {
            { new Vector2Int(0, 1) },
            { new Vector2Int(1, 1) },
            { new Vector2Int(1, 0) },
            { new Vector2Int(1, -1) },
            { new Vector2Int(0, -1) },
            { new Vector2Int(-1, -1) },
            { new Vector2Int(-1, 0) },
            { new Vector2Int(-1, 1) }
        };
        foreach (var direction in directions)
        {
            var neighbourTile = GridManager.Instance.GetTileAtPosition(unit.OccupiedTile.Position + direction);
            if (neighbourTile != null)
            {
                if (neighbourTile.OccupiedUnit != null)
                {
                    if (neighbourTile.OccupiedUnit.Side != unit.Side)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool IsRangeAttackPossible(BaseUnit unit)
    {
        if (!unit.abilities.Contains(Ability.Archer))
        {
            return false;
        }
        if (ShiftButtonHandler.Instance.isLeftShiftPressed)
        {
            return false;
        }
        if (unit.UnitArrows <= 0 || unit.UnitArrows == null)
        {
            return false;
        }
        if (IsEnemyAround(unit))
        {
            return false;
        }
        return true;
    }
    public void SetAdditionalDefend()
    {
        if (GameManager.Instance.CurrentSide == Side.Player && GameManager.Instance.PlayerFaction != null
            && GameManager.Instance.GameDifficulty != null)
        {
            BaseUnit unit = TurnManager.Instance.ATB.FirstOrDefault().Value;
            unit.UnitAdditionalDefence = (int)(unit.UnitDefence * 0.3);
            string message = $"<color=red>{unit.UnitName}</color> оборона.";
            MenuManager.Instance.AddMessageToChat(message);
            Tile.Instance.DeleteHighlight();
            TurnManager.Instance.EndTurn(unit);
        }
    }
    public void Wait()
    {
        if (GameManager.Instance.CurrentSide == Side.Player && GameManager.Instance.PlayerFaction != null 
            && GameManager.Instance.GameDifficulty != null)
        {
            BaseUnit unit = TurnManager.Instance.ATB.FirstOrDefault().Value;

            string message = $"<color=red>{unit.UnitName}</color> Ожидает.";
            Tile.Instance.DeleteHighlight();
            MenuManager.Instance.AddMessageToChat(message);

            TurnManager.Instance.WaitOrMoraleUnit(unit);

            unit.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if (GameManager.Instance.GameState == GameState.PlayerTurn)
            {
                SetSelectedHero(null);
            }

            TurnManager.Instance.StartTurn(TurnManager.Instance.ATB.FirstOrDefault().Value);
        }
    }
    public void CalculateProbabilityStats(BaseUnit unit)
    {
        unit.UnitProbabilityLuck = math.pow(unit.UnitLuck / 10.0, 1 + unit.UnitSuccessfulLuck - unit.UnitFailedLuck * (unit.UnitLuck / 10.0 / (1 - unit.UnitLuck / 10.0)));
        unit.UnitProbabilityMorale = math.pow(unit.UnitMorale / 10.0, 1 + unit.UnitSuccessfulMorale - unit.UnitFailedMorale * (unit.UnitMorale / 10.0 / (1 - unit.UnitMorale / 10.0)));
    }
    public bool Morale(BaseUnit unit)
    {
        var randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= unit.UnitProbabilityMorale)
        {
            UnitFactory.Instance.CreateMoraleEffect(unit).Forget();

            string color = unit.Side == Side.Player ? "red" : "blue";
            string message = $"<color={color}>{unit.UnitName}</color> рвется в бой!";
            MenuManager.Instance.AddMessageToChat(message);

            unit.UnitSuccessfulMorale += 1;
            TurnManager.Instance.WaitOrMoraleUnit(unit);

            unit.GetComponent<SpriteRenderer>().sortingOrder = 1;
            if (GameManager.Instance.GameState == GameState.PlayerTurn)
            {
                SetSelectedHero(null);
            }

            if (SpawnManager.Instance.EnemyUnits.Count == 0)
            {
                GameManager.Instance.ChangeState(GameState.GameOver);
                MenuManager.Instance.WinPanel();
            }
            else if (SpawnManager.Instance.PlayerUnits.Count == 0)
            {
                GameManager.Instance.ChangeState(GameState.GameOver);
                MenuManager.Instance.LosePanel();
            }
            else
            {
                TurnManager.Instance.StartTurn(TurnManager.Instance.ATB.FirstOrDefault().Value);
            }

            return true;
        }
        else
        {
            unit.UnitFailedMorale += 1;
            return false;
        }
    }
    public bool Luck(BaseUnit unit)
    {
        var randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= unit.UnitProbabilityLuck)
        {
            UnitFactory.Instance.CreateLuckEffect(unit).Forget();

            string color = unit.Side == Side.Player ? "red" : "blue";
            string message = $"<color={color}>{unit.UnitName}</color> посетила удача!";
            MenuManager.Instance.AddMessageToChat(message);

            unit.UnitSuccessfulLuck += 1;
            return true;
        }
        else
        {
            unit.UnitFailedLuck += 1;
            return false;
        }
    }

    public void KillAllUnits(BaseUnit attacker, IDamage damage)
    {
        foreach (var unit in TurnManager.Instance.allUnits.Where(u => u.Faction != Faction.Rory))
        {
            _ = unit.TakeMeleeDamage(attacker, unit, damage);
            _ = unit.Death();
        }
    }
}