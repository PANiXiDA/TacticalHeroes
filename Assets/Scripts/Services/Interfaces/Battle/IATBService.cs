using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;

using R3;

using System;
using System.Collections.Generic;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IATBService
    {
        Observable<IReadOnlyList<GameEntity>> OnAtbGenerated { get; }
        Observable<IReadOnlyList<ATBItem>> OnTurnOrderGenerated { get; }
        double GetGameObjectAtbPosition(Guid id);
        List<GameEntity> GetAtb();
        void SetAtb(List<GameObject> gameObjects);
        ATBNextTurnResult GetNextTurn(List<GameObject> gameObjects, List<GameEntity> atb);
        void UpdateAtb(Guid activeGameObjectId, bool isWait = false, double shiftFactor = 0.5);
        void PredictNextTurns();
    }
}
