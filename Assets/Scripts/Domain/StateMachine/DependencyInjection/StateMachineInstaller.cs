using System.Collections.Generic;
using System;

using Zenject;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.StateMachine.Interfaces.States;
using Assets.Scripts.Domain.StateMachine.Interfaces;
using Assets.Scripts.Domain.StateMachine.Implementations;
using Assets.Scripts.Domain.StateMachine.Implementations.States;

namespace Assets.Scripts.Domain.StateMachine.DependencyInjection
{
    public sealed class StateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IBattleStateMachine>().To<BattleStateMachine>().AsSingle();

            Container.Bind<IGameState>().WithId(GameState.GenerateGrid).To<GenerateGridState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.Spawn).To<SpawnState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.SetATB).To<SetATBState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.StartTurn).To<StartTurnState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.WaitAction).To<WaitActionState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.ApplyAction).To<ApplyActionState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.CheckBattleEnd).To<CheckBattleEndState>().AsSingle();
            Container.Bind<IGameState>().WithId(GameState.BattleEnd).To<BattleEndState>().AsSingle();

            Container.Bind<IReadOnlyDictionary<GameState, IGameState>>().FromMethod(ctx => BindBattleStates(ctx)).AsSingle();
        }

        private Dictionary<GameState, IGameState> BindBattleStates(InjectContext context)
        {
            var dictionary = new Dictionary<GameState, IGameState>();
            foreach (GameState gameState in Enum.GetValues(typeof(GameState)))
            {
                if (gameState == GameState.Default)
                {
                    continue;
                }
                dictionary[gameState] = context.Container.ResolveId<IGameState>(gameState);
            }

            return dictionary;
        }
    }
}
