using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Presenters;

using Zenject;

namespace Assets.Scripts.UI.Battle.Core
{
    public sealed class SceneContext : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EntryPoint>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GlobalInput>().FromComponentInHierarchy().AsSingle();
            Container.Bind<KeyboardInput>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GridsPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SpawnsPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ATBPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<AttackPreviewsPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<TileInfoPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<UnitInfoPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SurrenderPresenter>().FromComponentInHierarchy().AsSingle();
        }
    }
}
