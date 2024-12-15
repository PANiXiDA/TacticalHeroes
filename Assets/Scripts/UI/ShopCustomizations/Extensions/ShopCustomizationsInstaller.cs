using Zenject;

namespace Assets.Scripts.UI.ShopCustomizations.Extensions
{
    public class ShopCustomizationsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ShopCustomizationsManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ShopAvatarsManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ShopFramesManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
