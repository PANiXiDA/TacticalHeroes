using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Requests.ImageService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI
{
    public abstract class BaseProfileScene : MonoBehaviour
    {
        [SerializeField]
        protected GameObject _playerFrame, _playerAvatar;

        protected PlayerProfile _playerProfile;
        protected IImagesService _imagesService;

        [Inject]
        public void Construct(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        protected async UniTask InitializePlayerProfileAsync()
        {
            _playerProfile = PlayerProfile.Instance;

            _playerFrame.SetActive(true);
            _playerAvatar.SetActive(true);

            await LoadPlayerImageAsync(_playerProfile.Frame.FileName, _playerProfile.Frame.S3Path, _playerFrame);
            await LoadPlayerImageAsync(_playerProfile.Avatar.FileName, _playerProfile.Avatar.S3Path, _playerAvatar);
        }

        private async UniTask LoadPlayerImageAsync(string fileName, string s3Path, GameObject targetObject)
        {
            if (targetObject == null) return;

            await TaskRunner.RunWithGlobalErrorHandling(async () =>
            {
                var request = new LoadImageRequest(fileName, s3Path);
                var sprite = await _imagesService.LoadImage(request);

                var imageComponent = targetObject.GetComponentInChildren<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = sprite;
                }
            });
        }
    }
}
