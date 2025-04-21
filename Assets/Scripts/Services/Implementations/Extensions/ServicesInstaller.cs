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

            Container.Bind<IBattleStateMachine>().To<BattleStateMachine>().AsSingle();
            Container.Bind<IGridService>().To<GridService>().AsSingle();
        }
    }
}
