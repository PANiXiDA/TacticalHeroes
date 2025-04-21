using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;
using Assets.Scripts.Services.Interfaces.Battle.States.Core;

using Cysharp.Threading.Tasks;

using System.Collections.Generic;

using Zenject;

namespace Assets.Scripts.Services.Implementations.Battle.States.Core
{
    public sealed class BattleStateMachine : IBattleStateMachine
    {
        private readonly Dictionary<GameState, IGameState> _states;
        public GameState Current { get; private set; }

        [Inject]
        public BattleStateMachine(IGridService gridService)
        {
            _states = new()
            {
                { GameState.GenerateGrid, new GenerateGridState(gridService) },
            };
        }

        public async UniTask ChangeStateAsync(GameState next)
        {
            if (Current == next)
            {
                return;
            }
            if (_states.TryGetValue(Current, out var cur))
            {
                cur.Exit();
            }

            Current = next;
            await _states[Current].EnterAsync();
        }
    }
}
