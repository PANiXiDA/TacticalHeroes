using System;
using System.Collections.Generic;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.GameEngine.DTO.ATBCalculator;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;

using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Services.Interfaces.Battle
{
    public interface IBattleTurnsService
    {
        Observable<Guid> OnTurnStarted { get; }
        UniTask StartNextTurnAsync(List<GameObject> gameObjects, List<GameEntity> atb);
        UniTask CompleteTurnAsync(RoundState roundState, List<RoundState> gameHistory);
    }
}
