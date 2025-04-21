using Assets.Scripts.UI.Battle.Core;
using Assets.Scripts.UI.Battle.Presenters;

using Zenject;

namespace Assets.Scripts.UI.LoadBattle.Extensions
{
    public class LoadBattleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EntryPoint>().FromComponentInHierarchy().AsSingle();
            Container.Bind<GridPresenter>().FromComponentInHierarchy().AsSingle();
        }
    }
}
