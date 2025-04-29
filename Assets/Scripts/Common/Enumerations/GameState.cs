namespace Assets.Scripts.Common.Enumerations
{
    public enum GameState
    {
        Default = 0,
        GenerateGrid = 1,
        Spawn = 2,
        SetATB = 3,
        StartTurn = 4,
        WaitAction = 5,
        ApplyAction = 6,
        CheckBattleEnd = 7,
        BattleEnd = 8
    }
}