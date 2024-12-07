using Zenject;

namespace Assets.Scripts.UI.Profile.Extensions
{
    public class ProfileSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ProfileSceneManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
