using UnityEngine;
using System;
using Assets.Scripts.Enumerations;

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
                UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.SetATB:
                UnitManager.Instance.SetATB();
                break;
            case GameState.HeroesTurn:
                CurrentFaction = Faction.Hero;
                break;
            case GameState.EnemiesTurn:
                CurrentFaction = Faction.Enemy;
                await ArtificialIntelligence.Instance.Waiter();
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

}
