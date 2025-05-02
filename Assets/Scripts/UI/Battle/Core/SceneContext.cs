using Assets.Scripts.UI.Battle.Presenters;

using Zenject;

namespace Assets.Scripts.UI.Battle.Core
{
    public class SceneContext : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EntryPoint>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GridsPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<SpawnsPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ATBPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<AttackPreviewsPresenter>().FromComponentInHierarchy().AsSingle();
            Container.Bind<TileInfoPresenter>().FromComponentInHierarchy().AsSingle();
        }
    }
}
