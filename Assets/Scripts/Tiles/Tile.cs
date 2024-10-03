using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Managers;

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

    private static Tile currentTileUnderMouse;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (UnitManager.Instance.SelectedHero != null)
        {
            if (UnitManager.Instance.SelectedHero.isBusy)
            {
                flag = false;
            }
        }
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
        if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            if (currentTileUnderMouse == this)
            {
                MenuManager.Instance.ShowUnitInfo(this);
            }
        }
    }
    void OnMouseEnter()
    {
        currentTileUnderMouse = this;

        MenuManager.Instance.ShowTileInfo(this);

        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Side == Side.Enemy && UnitManager.Instance.SelectedHero != null)
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
        if (currentTileUnderMouse == this)
        {
            currentTileUnderMouse = null;
        }

        flag = false;
        MenuManager.Instance.DeleteSwords();
        MenuManager.Instance.DeleteArrows();
        DeleteHighlightForAttack();
    }
    private async void OnMouseDown()
    {
        if (IsPointerOverUIObject()) return;

        if (GameManager.Instance.GameState != GameState.PlayerTurn) return;

        BaseUnit attacker = UnitManager.Instance.SelectedHero;
        BaseUnit defender = OccupiedUnit;
        var rangeAttack = UnitManager.Instance.IsRangeAttackPossible(attacker);
        Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(attacker);

        if (attacker.isBusy) return;

        if (defender != null)
        {
            if (attacker != null && defender.Side != Side.Player &&
                defender != attacker)
            {
                if (rangeAttack)
                {
                    await attacker.RangeAttack(attacker, defender);
                }
                else
                {
                    targetTile = ShowTileForAttack(GridManager.Instance.GetTileCoordinate(this));
                    if (targetTile != null)
                    {
                        await attacker.MeleeAttack(attacker, defender, targetTile);
                    }
                }
            }
        }
        else
        {
            if (attacker != null && Walkable && tilesForMove.ContainsValue(this))
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
    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return false;
    }
}
