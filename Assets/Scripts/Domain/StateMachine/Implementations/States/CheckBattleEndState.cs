using System.Linq;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces.States;
using Assets.Scripts.Infrastructure.Models;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Domain.StateMachine.Implementations.States
{
    public sealed class CheckBattleEndState : IGameState
    {
        public CheckBattleEndState() { }

        public UniTask<GameState?> EnterAsync(GameSession gameSession)
        {
            var unitsBySide = gameSession.Players
                .GroupBy(player => player.Side)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(player => player.Units?.Count ?? 0)
                );

            var sideWithNoUnits = unitsBySide
                .FirstOrDefault(unitBySide => unitBySide.Value == 0)
                .Key;

            if (unitsBySide[sideWithNoUnits] == 0)
            {
                return UniTask.FromResult<GameState?>(GameState.BattleEnd);
            }

            return UniTask.FromResult<GameState?>(GameState.StartTurn);
        }

        public void Exit() { }
    }
}
