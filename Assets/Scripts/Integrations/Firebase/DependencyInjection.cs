using Assets.Scripts.Integrations.Firebase.Implementations;
using Assets.Scripts.Integrations.Firebase.Interfaces;
using Zenject;

public class UntitledInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IFirebaseAuthService>().To<FirebaseAuthService>().AsSingle();
        Container.Bind<IFirebaseRegistrationService>().To<FirebaseRegistrationService>().AsSingle();
        Container.Bind<IFirestoreUserService>().To<FirestoreUserService>().AsSingle();
    }
}