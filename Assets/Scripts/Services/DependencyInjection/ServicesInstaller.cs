using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Implementations.Battle;
using Assets.Scripts.Services.Implementations.Battle.States.Core;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.Services.Interfaces.Battle;
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

            Container.Bind<IBattleStateMachine>().To<BattleStateMachine>().AsSingle();
            Container.Bind<IGridsService>().To<GridsService>().AsSingle();
            Container.Bind<IUnitsService>().To<UnitsService>().AsSingle();
            Container.Bind<IBuildsService>().To<BuildsService>().AsSingle();
            Container.Bind<IBattlePreparationsService>().To<BattlePreparationsService>().AsSingle();
            Container.Bind<IATBService>().To<ATBService>().AsSingle();
        }
    }
}
