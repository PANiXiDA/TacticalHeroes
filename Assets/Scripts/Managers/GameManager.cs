using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    //����������� ����, ������, �������� ��������� �� ����� ������/���� ���������
    //� ��� ��������� � �������� ������ ������������� ��������� ��������� ������ / ���������.
    public static GameManager Instance;
    //instance - ��� ��������� �������. � ������ ������ ���������� ������� Singleton,
    //����������� ������� ����� ���� ��������� ���������� ���� ��� ����� ����������. 

    public GameState GameState;

    //public static event Action<GameState> OnGameStateChanged;

    //Awake ���� ����� ���������� ���� ��� ��� ������� ������� ��� ��� ������ �������������.
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }
    public void ChangeState(GameState newState)
    {
        GameState = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.HeroesTurn:
                break;
            case GameState.EnemiesTurn:
                StartCoroutine(ArtificialIntelligence.Instance.Waiter());
                //ArtificialIntelligence.Instance.waiter();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        //OnGameStateChanged?.Invoke(newState);
    }

}

//enum - ������������. ������������ ������������ ����� ��������� ��������� ��������.
public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4
}
