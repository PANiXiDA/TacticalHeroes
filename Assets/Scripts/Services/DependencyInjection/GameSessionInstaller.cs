using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Services.Implementations;
using Assets.Scripts.Services.Interfaces;

using Zenject;

namespace Assets.Scripts.Services.DependencyInjection
{
    public sealed class GameSessionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameSessionsFactory>().To<GameSessionsFactory>().AsSingle();
            Container.Bind<GameSession>().FromMethod(ctx => ctx.Container.Resolve<IGameSessionsFactory>().CreateDefault());
        }
    }
}
