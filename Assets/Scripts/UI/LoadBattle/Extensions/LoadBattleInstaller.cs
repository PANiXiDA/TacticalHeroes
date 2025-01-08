using Zenject;

namespace Assets.Scripts.UI.LoadBattle.Extensions
{
    public class LoadBattleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LoadBattleManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
