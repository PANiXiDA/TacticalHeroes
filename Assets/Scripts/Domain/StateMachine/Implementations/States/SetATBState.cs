using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces.States;
using Assets.Scripts.GameEngine.Domain.Core;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Cysharp.Threading.Tasks;

using R3;

namespace Assets.Scripts.Domain.StateMachine.Implementations.States
{
    public sealed class SetATBState : IGameState
    {
        private readonly IATBService _atbService;

        public SetATBState(IATBService atbService)
        {
            _atbService = atbService;
        }

        public async UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            var gameObjects = new List<GameObject>();
            gameObjects.AddRange(gameSession.RoundState.Units);
            gameObjects.AddRange(gameSession.RoundState.Heroes);

            var atbTask = _atbService.OnAtbGenerated.FirstAsync();
            _atbService.SetAtb(gameObjects);
            var atb = await atbTask;
            gameSession.RoundState.ATB = atb.ToList();
            return GameState.StartTurn;
        }

        public void Exit() { }
    }
}
