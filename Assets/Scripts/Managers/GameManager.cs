using UnityEngine;
using System;
using Assets.Scripts.Enumerations;
using Assets.Scripts.Managers;
using Assets.Scripts.Controllers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public Faction CurrentFaction;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }
    public async void ChangeState(GameState newState)
    {
        GameState = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnHeroes:
                SpawnManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                SpawnManager.Instance.SpawnEnemies();
                break;
            case GameState.SetATB:
                TurnManager.Instance.SetATB();
                break;
            case GameState.HeroesTurn:
                CurrentFaction = Faction.Hero;
                break;
            case GameState.EnemiesTurn:
                CurrentFaction = Faction.Enemy;
                await AIController.Instance.ExecuteAITurn();
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

}
