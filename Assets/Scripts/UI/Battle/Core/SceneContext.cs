using Assets.Scripts.UI.LoadBattle;

using Zenject;

namespace Assets.Scripts.UI.Battle.Core
{
    public class SceneContext : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LoadBattleManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
