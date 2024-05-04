using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    //—татические пол€, методы, свойства относ€тс€ ко всему классу/всей структуре
    //и дл€ обращени€ к подобным членам необ€зательно создавать экземпл€р класса / структуры.
    public static GameManager Instance;
    //instance - это экземпл€р объекта. ¬ данном случае реализован паттерн Singleton,
    //позвол€ющий создать всего один экземпл€р указанного типа дл€ всего приложени€. 

    public GameState GameState;

    //public static event Action<GameState> OnGameStateChanged;

    //Awake Ётот метод вызываетс€ один раз дл€ каждого объекта при его первой инициализации.
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

//enum - перечисление. ѕеречислени€ представл€ют набор логически св€занных констант.
public enum GameState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4
}
