using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

//���������� ������������ ������ ����������� � ��������������
//������ ����������� ��� �������� ������, ������� ����� ��������� ������������ ��������� ����������� �������.
public class Tile : MonoBehaviour
{
    public static Tile Instance;

    public string TileName;
    //public - ������ �� ���������;
    //protected - ������ ��������� ������ ������������ ��������;
    //internal - ������ ��������� ������� �������� �������;
    //private - ������ ��������� ������� ������� ������.
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight, _highlight_for_attack;
    [SerializeField] private bool _isWalkable; //����� �� �� ���� ������ ������

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public Vector2 Position;
    public Vector2 TargetPosition;
    public Tile PreviousNode;
    public float F; // F = G+H
    public float G; // ���������� �� ������ �� ������� ������
    public float H; // ���������� �� ������� �� ����

    public bool flag = false; // �������� ������� �� ������ ���� �� �����

    public Tile TileForAttackMove = null; // ������, � ������� �� ������ ������������� ��� �����

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

    ////������ � ��������, ������� �� ����� ������� ���������� ��� ���������������, � ������� ������ ���������� ������������� virtual.
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
                SetHighlight(UnitManager.Instance.SelectedHero); // ������� ��������� ��� ���������� ���� �����
            }
            else
            {
                if (UnitManager.Instance.SelectedHero != null && OccupiedUnit.Faction != Faction.Hero)
                {
                    TileForAttackMove = ShowTileForAttack(GridManager.Instance.GetTileCoordinate(this));
                    StartCoroutine(UnitManager.Instance.SelectedHero.Attack(this, TileForAttackMove));
                    // ����� ���������� ����� � �������� ������������
                    //DeleteHighlight();// ������� ��������� ����� ����� ���������� �����
                }
            }
        }
        else
        {
            if (UnitManager.Instance.SelectedHero != null)
            {
                UnitManager.Instance.SelectedHero.Move(this, true); // ���������� ���������� ����� �� ������, �� ������� ��������
                //DeleteHighlight(); // ������� ��������� ����� ����������� �����
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
