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
        Observable<(IReadOnlyList<ATBItem> Items, Guid HighlightId)> OnTurnOrderPreview { get; }
        Observable<Unit> OnCancelTurnOrderPreview { get; }
        double GetGameObjectAtbPosition(Guid id);
        bool TryGetGameObject(Guid id, out GameObject gameObject);
        List<GameEntity> GetAtb();
        ATBNextTurnResult GetNextTurn(List<GameObject> gameObjects, List<GameEntity> atb);
        void SetAtb(List<GameObject> gameObjects);
        void UpdateAtb(Guid activeGameObjectId, bool isWait = false, double shiftFactor = 0.5);
        void PredictNextTurns();
        void BuildWaitPreview(Guid activeGameObjectId, bool isWait = false, double shiftFactor = 0.5);
        void CancelWaitPreview();
    }
}
