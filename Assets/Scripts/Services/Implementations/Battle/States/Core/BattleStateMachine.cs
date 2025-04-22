using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;
using Assets.Scripts.Services.Interfaces.Battle.States.Core;

using Cysharp.Threading.Tasks;

using R3;

using System.Collections.Generic;

namespace Assets.Scripts.Services.Implementations.Battle.States.Core
{
    public sealed class BattleStateMachine : IBattleStateMachine
    {
        private readonly Dictionary<GameState, IGameState> _states;
        private readonly CompositeDisposable _disposables = new();

        public GameState Current { get; private set; }

        public BattleStateMachine(
            GameSession gameSession,
            IGridsService gridService,
            IUnitsService unitsService)
        {
            _states = new()
            {
                { GameState.GenerateGrid, new GenerateGridState(gridService) },
                { GameState.SpawnUnits, new SpawnUnitsState(gameSession, unitsService) },
            };

            gridService.OnGridGenerated
                .Subscribe(_ => ChangeStateAsync(GameState.SpawnUnits).Forget())
                .AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();

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
