using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Assets.Scripts.Domain.GameEngine.DTO.Extensions;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;
using System;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class ATBService : IATBService
    {
        private readonly IATBCalculator _atbCalculator;

        private readonly Subject<IReadOnlyList<GameEntity>> _atbGenerated = new();
        private readonly Subject<IReadOnlyList<ATBItem>> _turnOrderGenerated = new();

        private List<GameEntity> _atb = new();

        private List<GameObject> _gameObject = new();
        private List<GameEntityInitiative> _initiatives = new();

        public Observable<IReadOnlyList<GameEntity>> OnAtbGenerated => _atbGenerated.AsObservable();
        public Observable<IReadOnlyList<ATBItem>> OnTurnOrderGenerated => _turnOrderGenerated.AsObservable();

        public ATBService(IATBCalculator aTBCalculator) 
        {
            _atbCalculator = aTBCalculator;
        }

        public double GetGameObjectAtbPosition(Guid id) => _atb.FirstOrDefault(item => item.GameEntityId == id).Position;
        public List<GameEntity> GetAtb() => _atb;

        public void SetAtb(List<GameObject> gameObjects)
        {
            var initiatives = gameObjects
                .Select(gameObject => new GameEntityInitiative(gameObject.Id, (gameObject as Unit)?.EffectiveInitiative() ?? gameObject.Initiative))
                .ToList();

            var atb = _atbCalculator.SetStartingPosition(initiatives);
            _atbGenerated.OnNext(atb);

            SetAtbCache(atb);
            SetGameObjectsCache(gameObjects, initiatives);

            PredictNextTurns();
        }

        public ATBNextTurnResult GetNextTurn(List<GameObject> gameObjects, List<GameEntity> atb)
        {
            var initiatives = gameObjects
                .Select(gameObject => new GameEntityInitiative(gameObject.Id, gameObject.Initiative))
                .ToList();

            var currentSnapshotAtb = atb.Select(item => new GameEntity
            {
                GameEntityId = item.GameEntityId,
                Position = item.Position
            }).ToList();

            var result = _atbCalculator.GetNextTurn(new ATBCalculationContext()
            {
                GameEntitiesInitiatives = initiatives,
                CurrentATBState = currentSnapshotAtb
            });

            SetAtbCache(result.UpdatedATBState);
            SetGameObjectsCache(gameObjects, initiatives);

            return result;
        }

        public void UpdateAtb(Guid activeGameObjectId, bool isWait = false, double shiftFactor = 0.5)
        {
            _atbCalculator.ShiftATBPosition(new ATBPositionShiftContext
            {
                CurrentATBState = _atb,
                GameObjectId = activeGameObjectId,
                ShiftFactor = shiftFactor,
                IsWait = isWait
            });

            SetAtbCache(_atb);
        }

        public void PredictNextTurns()
        {
            var byId = _gameObject.ToDictionary(gameObject => gameObject.Id);

            var turnOrder = _atbCalculator.PredictNextTurns(
                new ATBCalculationContext
                {
                    GameEntitiesInitiatives = _initiatives,
                    CurrentATBState = _atb
                },
                countTurns: 300);

            var atbItems = turnOrder.Select(id =>
            {
                var obj = byId[id];
                int? count = (obj as Unit)?.Count;

                return new ATBItem(
                    id: obj.Id,
                    name: obj.Name,
                    playerId: obj.OwnerId.GetValueOrDefault(),
                    count: count);
            }).ToList();

            _turnOrderGenerated.OnNext(atbItems);
        }

        private void SetAtbCache(List<GameEntity> atb)
        {
            _atb = atb;
        }

        private void SetGameObjectsCache(List<GameObject> gameObjects, List<GameEntityInitiative> initiatives)
        {
            _gameObject = gameObjects;
            _initiatives = initiatives;
        }
    }
}
