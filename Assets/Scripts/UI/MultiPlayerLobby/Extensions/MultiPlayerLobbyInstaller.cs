using Zenject;

namespace Assets.Scripts.UI.MultiPlayerLobby.Extensions
{
    public class MultiPlayerLobbyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ProfileManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<ChatManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayManager>().FromComponentInHierarchy().AsSingle();
        }
    }
}