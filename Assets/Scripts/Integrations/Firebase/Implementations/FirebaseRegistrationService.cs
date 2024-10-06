using Assets.Scripts.Integrations.Firebase.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using System.IO;
using UnityEngine;

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
                var imageUrl = await UploadProfileImage(user.UserId, "DefaultProfile");

                UserProfile profile = new UserProfile
                {
                    DisplayName = nickname,
                    PhotoUrl = new System.Uri(imageUrl)
                };

                await user.UpdateUserProfileAsync(profile);
                await user.SendEmailVerificationAsync();
            }

            return user.UserId;
        }

        public async UniTask<string> UploadProfileImage(string userId, string fileName)
        {
            string imagePath = Path.Combine(Application.streamingAssetsPath, "Sprites", "UI", "HeroPortrets", $"{fileName}.png");

            byte[] imageData;

            #if UNITY_ANDROID && !UNITY_EDITOR
                UnityWebRequest request = UnityWebRequest.Get(imagePath);
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new FileNotFoundException($"Ошибка загрузки файла: {request.error}");
                }

                imageData = request.downloadHandler.data;
            #else
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Файл не найден: {imagePath}");
            }

            imageData = File.ReadAllBytes(imagePath);
            #endif

            var storage = FirebaseStorage.DefaultInstance;
            var storageRef = storage.GetReference($"profile_images/{userId}.png");

            var metadata = new MetadataChange();
            metadata.ContentType = "image/png";

            var uploadTask = storageRef.PutBytesAsync(imageData, metadata);
            await uploadTask;

            var downloadUrl = await storageRef.GetDownloadUrlAsync();
            return downloadUrl.ToString();
        }
    }
}
