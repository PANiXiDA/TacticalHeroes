using Assets.Scripts.Services.Interfaces;
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
        }
    }
}
