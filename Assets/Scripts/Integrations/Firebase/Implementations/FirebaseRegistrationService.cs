using Assets.Scripts.Integrations.Firebase.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase.Auth;

namespace Assets.Scripts.Integrations.Firebase.Implementations
{
    internal class FirebaseRegistrationService : IFirebaseRegistrationService
    {
        private FirebaseAuth auth;

        public FirebaseRegistrationService()
        {
            auth = FirebaseAuth.DefaultInstance;
        }

        public async UniTask<string> Register(string email, string password, string nickname)
        {
            var registerTask = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var user = registerTask.User;

            if (user != null)
            {
                UserProfile profile = new UserProfile { DisplayName = nickname };
                await user.UpdateUserProfileAsync(profile);

                await user.SendEmailVerificationAsync();
            }

            return user.UserId;
        }
    }
}
