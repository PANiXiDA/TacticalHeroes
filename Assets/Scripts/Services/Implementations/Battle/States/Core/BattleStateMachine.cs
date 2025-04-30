using Assets.Scripts.Common.Enumerations;
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
        private readonly GameSession _gameSession;

        private readonly CompositeDisposable _disposables = new();

        public GameState Current { get; private set; }

        public BattleStateMachine(
            GameSession gameSession,
            IGridsService gridService,
            IBattlePreparationsService spawnersService,
            IATBService atbService,
            IBattleTurnsService battleTurnsService,
            IMovementsService movementsService,
            IAttacksService attacksService)
        {
            _gameSession = gameSession;

            _states = new()
            {
                { GameState.GenerateGrid, new GenerateGridState(gridService) },
                { GameState.Spawn, new SpawnState(spawnersService) },
                { GameState.SetATB, new SetATBState(atbService) },
                { GameState.StartTurn, new StartTurnState(battleTurnsService, movementsService) },
                { GameState.WaitAction, new WaitActionState(movementsService, attacksService) },
                { GameState.ApplyAction, new ApplyActionState(battleTurnsService) },
                { GameState.CheckBattleEnd, new CheckBattleEndState() },
                { GameState.BattleEnd, new BattleEndState() },
            };
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

            var nextState = await _states[Current].EnterAsync(_gameSession);
            if (nextState.HasValue)
            {
                await ChangeStateAsync(nextState.Value);
            }
        }
    }
}
