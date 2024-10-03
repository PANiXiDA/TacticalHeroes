using UnityEngine;
using System;
using Assets.Scripts.Managers;
using Assets.Scripts.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState GameState;
    public Side CurrentSide;
    public Faction? PlayerFaction;

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
            case GameState.BattleSettings:
                MenuManager.Instance.FactionChoosingPanelSetActive(true);
                break;
            case GameState.SpawnPlayerUnits:
                SpawnManager.Instance.SpawnPlayerUnits();
                break;
            case GameState.SpawnEnemyUnits:
                SpawnManager.Instance.SpawnEnemyUnits();
                break;
            case GameState.SetATB:
                TurnManager.Instance.SetATB();
                break;
            case GameState.PlayerTurn:
                CurrentSide = Side.Player;
                break;
            case GameState.EnemyTurn:
                CurrentSide = Side.Enemy;
                await AI.Instance.ExecuteAITurn();
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void SetPlayerFaction(int factionId)
    {
        PlayerFaction = (Faction)factionId;
        MenuManager.Instance.FactionChoosingPanelSetActive(false);
        MenuManager.Instance.SetHeroPortrets();
        SpawnManager.Instance.SetUnitsSettings();
    }
}
