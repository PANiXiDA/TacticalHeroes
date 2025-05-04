using System.Collections.Generic;
using System;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Implementations.Battle;
using Assets.Scripts.Services.Implementations.Battle.States;
using Assets.Scripts.Services.Implementations.Battle.States.Core;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.Services.Interfaces.Battle.States;
using Assets.Scripts.Services.Interfaces.Battle.States.Core;

using Zenject;

namespace Assets.Scripts.Services.Implementations.Extensions
{
    public class ServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IAuthService>().To<AuthService>().AsSingle();
            Container.Bind<IPlayersService>().To<PlayersService>().AsSingle();
            Container.Bind<IImagesService>().To<ImagesService>().AsSingle();
            Container.Bind<IAvatarsService>().To<AvatarsService>().AsSingle();
            Container.Bind<IFramesService>().To<FramesService>().AsSingle();
            Container.Bind<IChatsService>().To<ChatsService>().AsSingle();
            Container.Bind<IMatchmakingeService>().To<MatchmakingeService>().AsSingle();

            Container.Bind<IGameSessionsFactory>().To<GameSessionsFactory>().AsSingle();
            Container.Bind<GameSession>().FromMethod(ctx => ctx.Container.Resolve<IGameSessionsFactory>().CreateDefault());

            Container.Bind<IBattleActionsFacade>().To<BattleActionsFacade>().AsSingle();

            Container.Bind<IPlayerColorsService>().To<PlayerColorsService>().AsSingle();
            Container.Bind<IGridsService>().To<GridsService>().AsSingle();
            Container.Bind<IUnitsService>().To<UnitsService>().AsSingle();
            Container.Bind<IBuildsService>().To<BuildsService>().AsSingle();
            Container.Bind<IBattlePreparationsService>().To<BattlePreparationsService>().AsSingle();
            Container.Bind<IATBService>().To<ATBService>().AsSingle();
            Container.Bind<IAttacksService>().To<AttacksService>().AsSingle();
            Container.Bind<IMovementsService>().To<MovementsService>().AsSingle();
            Container.Bind<IBattleTurnsService>().To<BattleTurnsService>().AsSingle();
            Container.Bind<IButtonStatesService>().To<ButtonStatesService>().AsSingle();
            Container.Bind<IBuffsDebuffsService>().To<BuffsDebuffsService>().AsSingle();

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
