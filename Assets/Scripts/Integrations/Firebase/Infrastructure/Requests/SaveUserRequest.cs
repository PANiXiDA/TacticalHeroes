using Assets.Scripts.Integrations.Firebase.Infrastructure.Requests.Core;
using Firebase.Firestore;

namespace Assets.Scripts.Integrations.Firebase.Infrastructure.Requests
{
    [FirestoreData]
    public class SaveUserRequest : BaseRequest
    {
        [FirestoreProperty]
        public string UserAuthId { get; set; }

        [FirestoreProperty]
        public string Nickname { get; set; }

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public bool IsBlocked { get; set; }

        [FirestoreProperty]
        public bool IsEmailConfirmed { get; set; }
    }
}
