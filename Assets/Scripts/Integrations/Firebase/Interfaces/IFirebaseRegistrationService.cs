using Cysharp.Threading.Tasks;

namespace Assets.Scripts.Integrations.Firebase.Interfaces
{
    public interface IFirebaseRegistrationService
    {
        UniTask<string> Register(string email, string password, string nickname);
        UniTask<string> UploadProfileImage(string userId, string fileName);
    }
}
