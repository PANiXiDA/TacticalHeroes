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
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _path;
    [SerializeField] private bool _isWalkable; //можно ли по этой плитке ходить

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public Vector2 Position;
    public Vector2 TargetPosition;
    public Tile PreviousNode;
    public float F; // F = G+H
    public float G; // Расстояние от старта до текущей клетки
    public float H; // Расстояние от текущей до цели

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
    //private void Update(List<Vector2> path)
    //{
    //    //Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);
    //    foreach (var tile in path)
    //    {
    //        //while(UnitManager.Instance.SelectedHero.transform.position != GridManager.Instance.GetTileAtPosition(tile).transform.position)
    //        UnitManager.Instance.SelectedHero.transform.position = Vector3.MoveTowards(UnitManager.Instance.SelectedHero.transform.position, GridManager.Instance.GetTileAtPosition(tile).transform.position, Time.deltaTime * 10);
    //    }

    //}
    void OnMouseEnter()
    {
        MenuManager.Instance.ShowTileInfo(this);
    }
    void OnMouseExit()
    {
        MenuManager.Instance.ShowTileInfo(null);
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
                    var enemy = (BaseEnemy)OccupiedUnit;
                    var enemy_tile = GridManager.Instance.GetTileCoordinate(enemy.OccupiedTile);
                    var hero_tile = GridManager.Instance.GetTileCoordinate(UnitManager.Instance.SelectedHero.OccupiedTile);
                    if (Math.Abs(enemy_tile.x - hero_tile.x) <= 1 && Math.Abs(enemy_tile.y - hero_tile.y) <= 1)
                    {
                        var health = UnitManager.Instance.Attack(UnitManager.Instance.SelectedHero, enemy);
                        if (health <= 0)
                        {
                            SetHighlight(UnitManager.Instance.SelectedHero);// Убираем подсветку после атаки вражеского юнита
                            enemy.animator.Play("Death");
                            enemy.OccupiedTile.OccupiedUnit = null;
                            UnitManager.Instance.EnemyUnits.Remove(enemy);
                            //Destroy(enemy.gameObject);
                            UnitManager.Instance.SetSelectedHero(null);
                            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
                        }
                        else
                        {
                            SetHighlight(UnitManager.Instance.SelectedHero);// Убираем подсветку после атаки вражеского юнита
                            UnitManager.Instance.SetSelectedHero(null);

                            GameManager.Instance.ChangeState(GameState.EnemiesTurn);
                        }
                    }
                }
            }
        }
        else
        {
            if (UnitManager.Instance.SelectedHero != null)
            {
                Dictionary<Vector2, Tile> tilesForMove = UnitManager.Instance.GetTilesForMove(UnitManager.Instance.SelectedHero);
                if (tilesForMove.ContainsValue(this))
                {
                    SetHighlight(UnitManager.Instance.SelectedHero); // Убираем подсветку после перемещения юнита

                    var path = PathFinder.Instance.GetPath(GridManager.Instance.GetTileCoordinate(UnitManager.Instance.SelectedHero.OccupiedTile), GridManager.Instance.GetTileCoordinate(this));
                    path.Reverse();
                    path.RemoveAt(0);
                    Vector3[] path_ = new Vector3[path.Count];
                    for (int i = 0; i < path.Count; i++)
                    {
                        path_[i] = new Vector3(path[i].x, path[i].y, 0);
                    }
                    UnitManager.Instance.SelectedHero.animator.Play("Move");
                    UnitManager.Instance.SelectedHero.transform.DOPath(path_, 1, PathType.Linear, PathMode.TopDown2D)
                        .SetEase(Ease.Linear);

                    if (UnitManager.Instance.SelectedHero.OccupiedTile != null)
                    {
                        UnitManager.Instance.SelectedHero.OccupiedTile.OccupiedUnit = null;
                    }
                    this.OccupiedUnit = UnitManager.Instance.SelectedHero;
                    UnitManager.Instance.SelectedHero.OccupiedTile = this;

                    UnitManager.Instance.SetSelectedHero(null);
                    GameManager.Instance.ChangeState(GameState.EnemiesTurn);
                }
            }
        }
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
            else
            {
                t.Value._highlight.SetActive(false);
            }
        }
    }
}
