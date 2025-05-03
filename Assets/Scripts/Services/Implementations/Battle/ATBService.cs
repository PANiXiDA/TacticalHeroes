using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;

using Cysharp.Threading.Tasks;

using R3;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.Services.Implementations.Battle
{
    public sealed class ATBService : IATBService
    {
        private readonly IATBCalculator _atbCalculator;

        private readonly Subject<IReadOnlyList<GameEntity>> _atbGenerated = new();
        private readonly Subject<IReadOnlyList<ATBItem>> _turnOrderGenerated = new();

        public Observable<IReadOnlyList<GameEntity>> OnAtbGenerated => _atbGenerated.AsObservable();
        public Observable<IReadOnlyList<ATBItem>> OnTurnOrderGenerated => _turnOrderGenerated.AsObservable();

        public ATBService(IATBCalculator aTBCalculator) 
        {
            _atbCalculator = aTBCalculator;
        }

        public void SetAtb(List<GameObject> gameObjects)
        {
            var byId = gameObjects.ToDictionary(gameObject => gameObject.Id);

            var initiatives = gameObjects
                .Select(gameObject => new GameEntityInitiative(gameObject.Id, gameObject.Initiative))
                .ToList();

            var atb = _atbCalculator.SetStartingPosition(initiatives);
            _atbGenerated.OnNext(atb);

            var turnOrder = _atbCalculator.PredictNextTurns(
                new ATBCalculationContext
                {
                    GameEntitiesInitiatives = initiatives,
                    CurrentATBState = atb
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
    }
}
