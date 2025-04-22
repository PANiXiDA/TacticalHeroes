using Assets.Scripts.Repositories.Implementations.SQLite;
using Assets.Scripts.Repositories.Implementations.SQLite.Core;
using Assets.Scripts.Repositories.Interfaces;

using Zenject;

namespace Assets.Scripts.Repositories.DependencyInjection
{
    public class RepositoriesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DatabaseContext>().AsSingle().NonLazy();

            Container.Bind<IUnitsRepository>().To<UnitsRepository>().AsSingle();
        }
    }
}
