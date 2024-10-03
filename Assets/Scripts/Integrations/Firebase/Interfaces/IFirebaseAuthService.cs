using Cysharp.Threading.Tasks;
using Firebase.Auth;

namespace Assets.Scripts.Integrations.Firebase.Interfaces
{
    public interface IFirebaseAuthService
    {
        UniTask<FirebaseUser> SignIn(string email, string password);
        UniTask RecoveryPassword(string email);
    }
}
