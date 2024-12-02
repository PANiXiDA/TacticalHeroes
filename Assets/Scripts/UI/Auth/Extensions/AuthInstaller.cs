using Zenject;

namespace Assets.Scripts.UI.Auth.Extensions
{
    public class AuthInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<AuthManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}
