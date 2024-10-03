using Assets.Scripts.Integrations.Firebase.Infrastructure.Requests;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;

namespace Assets.Scripts.Integrations.Firebase.Interfaces
{
    public interface IFirestoreUserService
    {
        UniTask SaveUserAsync(SaveUserRequest userRequest);
        UniTask<DocumentSnapshot> GetUserByNicknameAsync(string nickname);
    }
}
