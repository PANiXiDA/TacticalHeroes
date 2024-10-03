using Firebase.Firestore;

namespace Assets.Scripts.Integrations.Firebase.Infrastructure.Requests.Core
{
    [FirestoreData]
    public class BaseRequest
    {
        [FirestoreProperty]
        public Timestamp CreatedAt { get; set; }

        [FirestoreProperty]
        public Timestamp UpdatedAt { get; set; }
    }
}
