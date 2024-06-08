using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
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
            case GameState.SetATB:
                UnitManager.Instance.SetATB();
                break;
            case GameState.HeroesTurn:
                break;
            case GameState.EnemiesTurn:
                StartCoroutine(ArtificialIntelligence.Instance.Waiter());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

}
public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    SetATB = 3,
    HeroesTurn = 4,
    EnemiesTurn = 5
}
