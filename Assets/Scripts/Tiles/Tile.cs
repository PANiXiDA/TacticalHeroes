using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enumeration;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Enumerations;

public class Tile : MonoBehaviour
{
    public static Tile Instance;

    public string TileName;

    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight, _highlight_for_attack, _highlight_hero;
    [SerializeField] private bool _isWalkable; 

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public Vector2 Position;
    public Vector2 TargetPosition;
    public Tile PreviousNode;
    public float F; // F = G+H
    public float G; // Расстояние от старта до текущей клетки
    public float H; // Расстояние от текущей до цели

    public bool flag = false; // проверка наведен ли курсор мыши на врага

    public Tile targetTile = null; // Клетка, в которую мы должны переместиться при атаке

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (flag == true)
        {
            MenuManager.Instance.ShowOrientationOfAttack(this);

            targetTile = ShowTileForAttack(GridManager.Instance.GetTileCoordinate(this));
            if (targetTile != null)
            {
                DeleteHighlightForAttack();
                targetTile._highlight_for_attack.SetActive(true);
            }
        }
    }
    void OnMouseEnter()
    {
        MenuManager.Instance.ShowTileInfo(this);
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.Enemy && UnitManager.Instance.SelectedHero != null)
            {
                flag = true;
            }
        }
        if (UnitManager.Instance.SelectedHero != null)
        {
            Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);
            if (Walkable && tilesForMove.ContainsValue(this))
                _highlight_for_attack.SetActive(true);
        }
    }
    void OnMouseExit()
    {
        MenuManager.Instance.ShowTileInfo(null);
        flag = false;
        MenuManager.Instance.DeleteSwords();
        MenuManager.Instance.DeleteArrows();
        DeleteHighlightForAttack();
    }
    private async void OnMouseDown()
    {
        BaseUnit attacker = UnitManager.Instance.SelectedHero;
        BaseUnit defender = OccupiedUnit;

        if (GameManager.Instance.GameState != GameState.HeroesTurn || attacker.isBusy) return;

        if (defender != null)
        {
            if (attacker != null && defender.Faction != Faction.Hero &&
                defender != attacker)
            {
                if (!attacker.abilities.Contains(Abilities.Archer))
                {
                    targetTile = ShowTileForAttack(GridManager.Instance.GetTileCoordinate(this));
                    await attacker.Attack(attacker, defender, targetTile);
                }
                else
                {
                    await attacker.RangeAttack(attacker, defender);
                }
            }
        }
        else
        {
            if (attacker != null && Walkable)
            {
                await attacker.Move(attacker, this);
            }
        }
    }
    public Tile ShowTileForAttack(Vector3 enemyPos)
    {
        Dictionary<string, Vector2Int> directions = new Dictionary<string, Vector2Int>()
        {
            { "Bottom", new Vector2Int(0, 1) },
            { "BottomLeft", new Vector2Int(1, 1) },
            { "Left", new Vector2Int(1, 0) },
            { "TopLeft", new Vector2Int(1, -1) },
            { "Top", new Vector2Int(0, -1) },
            { "TopRight", new Vector2Int(-1, -1) },
            { "Right", new Vector2Int(-1, 0) },
            { "BottomRight", new Vector2Int(-1, 1) }
        };

        foreach (var direction in directions)
        {
            if (GameObject.Find($"{direction.Key}(Clone)"))
            {
                Vector2 tilePos = new Vector2(enemyPos.x + direction.Value.x, enemyPos.y + direction.Value.y);
                return GridManager.Instance.GetTileAtPosition(tilePos);
            }
        }

        return null;
    }
    public void SetUnit(BaseUnit unit, Tile tile)
    {
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
        }
        unit.transform.position = tile.transform.position;
        tile.OccupiedUnit = unit;
        unit.OccupiedTile = tile;
    }
    public void SetHighlight(BaseUnit unit)
    {
        unit.OccupiedTile._highlight_hero.SetActive(true);
        var tilesForMove = UnitManager.Instance.GetTilesForMove(unit);
        foreach (var t in tilesForMove)
        {
            if (t.Value._highlight.activeInHierarchy == false)
            {
                t.Value._highlight.SetActive(true);
            }
        }
    }
    public void DeleteHighlight()
    {
        var Grid = GridManager.Instance.GetGrid();
        foreach (var t in Grid)
        {
            t.Value._highlight.SetActive(false);
            t.Value._highlight_for_attack.SetActive(false);
            t.Value._highlight_hero.SetActive(false);
        }
    }
    public void DeleteHighlightForAttack()
    {
        var Grid = GridManager.Instance.GetGrid();
        foreach (var t in Grid)
        {
            t.Value._highlight_for_attack.SetActive(false);
        }
    }
}
