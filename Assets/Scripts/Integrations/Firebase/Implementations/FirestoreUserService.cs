using Assets.Scripts.Integrations.Firebase.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Assets.Scripts.Integrations.Firebase.Infrastructure.Requests;

namespace Assets.Scripts.Integrations.Firebase.Implementations
{
    public class FirestoreUserService : IFirestoreUserService
    {
        FirebaseFirestore db;

        public FirestoreUserService()
        {
            db = FirebaseFirestore.DefaultInstance;
        }

        public async UniTask SaveUserAsync(SaveUserRequest userRequest)
        {
            await db.Collection("users").Document(userRequest.Nickname).SetAsync(userRequest);
        }

        public async UniTask<DocumentSnapshot> GetUserByNicknameAsync(string nickname)
        {
            DocumentReference docRef = db.Collection("users").Document(nickname);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot;
            }
            else
            {
                return null;
            }
        }
    }
}
