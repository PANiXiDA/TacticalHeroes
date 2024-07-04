using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enumerations
{
    public enum GameState
    {
        GenerateGrid = 0,
        SpawnHeroes = 1,
        SpawnEnemies = 2,
        SetATB = 3,
        HeroesTurn = 4,
        EnemiesTurn = 5,
        GameOver = 6
    }
}
