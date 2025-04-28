using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;

using R3;

using System.Collections.Generic;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IATBService
    {
        Observable<IReadOnlyList<GameEntity>> OnAtbGenerated { get; }
        Observable<IReadOnlyList<ATBItem>> OnTurnOrderGenerated { get; }
        void SetAtb(List<GameObject> gameObjects);
    }
}
