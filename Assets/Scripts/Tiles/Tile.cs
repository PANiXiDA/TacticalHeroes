using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

//Назначение абстрактного класса заключается в предоставлении
//общего определения для базового класса, которое могут совместно использовать несколько производных классов.
public class Tile : MonoBehaviour
{
    public static Tile Instance;

    public string TileName;
    //public - доступ не ограничен;
    //protected - доступ ограничен только наследуемыми классами;
    //internal - доступ ограничен рамками текущего проекта;
    //private - доступ ограничен рамками данного класса.
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight, _highlight_for_attack;
    [SerializeField] private bool _isWalkable; //можно ли по этой плитке ходить

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public Vector2 Position;
    public Vector2 TargetPosition;
    public Tile PreviousNode;
    public float F; // F = G+H
    public float G; // Расстояние от старта до текущей клетки
    public float H; // Расстояние от текущей до цели

    public bool flag = false; // проверка наведен ли курсор мыши на врага

    public Tile TileForAttackMove = null; // Клетка, в которую мы должны переместиться при атаке

    public void SetTile(float g, Vector2 nodePosition, Vector2 targetPosition, Tile previousNode)
    {
        Position = nodePosition;
        TargetPosition = targetPosition;
        PreviousNode = previousNode;
        G = g;
        float _min = Math.Min(Math.Abs(TargetPosition.x - Position.x), Math.Abs(TargetPosition.y - Position.y));
        float _max = Math.Max(Math.Abs(TargetPosition.x - Position.x), Math.Abs(TargetPosition.y - Position.y));
        H = (float)Math.Sqrt(2) * _min + _max - _min;
        F = G + H;
    }

    private void Awake()
    {
        Instance = this;
    }

    ////методы и свойства, которые мы хотим сделать доступными для переопределения, в базовом классе помечается модификатором virtual.
    public virtual void Init(int x, int y)
    {

    }
    void Update()
    {
        if (flag == true)
        {
            MenuManager.Instance.ShowOrientationOfAttack(this);

            TileForAttackMove = ShowTileForAttack(GridManager.Instance.GetTileCoordinate(this));
            if (TileForAttackMove != null)
                TileForAttackMove._highlight_for_attack.SetActive(true);
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
        DeleteHighlightForAttack();
    }
    private void OnMouseDown()
    {
        if (GameManager.Instance.GameState != GameState.HeroesTurn) return;
        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.Hero && UnitManager.Instance.SelectedHero == null)
            {
                UnitManager.Instance.SetSelectedHero((BaseHero)OccupiedUnit);
                SetHighlight(UnitManager.Instance.SelectedHero); // Создаем подсветку для выбранного нами героя
            }
            else
            {
                if (UnitManager.Instance.SelectedHero != null && OccupiedUnit.Faction != Faction.Hero)
                {
                    TileForAttackMove = ShowTileForAttack(GridManager.Instance.GetTileCoordinate(this));
                    StartCoroutine(UnitManager.Instance.SelectedHero.Attack(this, TileForAttackMove));
                    // Атака вражеского юнита в пределах досегаемости
                    //DeleteHighlight();// Убираем подсветку после атаки вражеского юнита
                }
            }
        }
        else
        {
            if (UnitManager.Instance.SelectedHero != null)
            {
                UnitManager.Instance.SelectedHero.Move(this, true); // Перемещаем выбранного юнита на клетку, на которую нажимаем
                //DeleteHighlight(); // Убираем подсветку после перемещения юнита
            }
        }
    }
    public Tile ShowTileForAttack(Vector3 enemyPos)
    {
        if (GameObject.Find("Bottom(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y + 1));
        }
        if (GameObject.Find("BottomLeft(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y + 1));
        }
        if (GameObject.Find("Left(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y));
        }
        if (GameObject.Find("TopLeft(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x + 1, enemyPos.y - 1));
        }
        if (GameObject.Find("Top(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x, enemyPos.y - 1));
        }
        if (GameObject.Find("TopRight(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y - 1));
        }
        if (GameObject.Find("Right(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y));
        }
        if (GameObject.Find("BottomRight(Clone)"))
        {
            return GridManager.Instance.GetTileAtPosition(new Vector2(enemyPos.x - 1, enemyPos.y + 1));
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
