using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Requests.ImageService;
using Assets.Scripts.Infrastructure.Responses.AvatarsService;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.UI.ShopCustomizations.Extensions;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Avatar = Assets.Scripts.Infrastructure.Models.Avatar;

namespace Assets.Scripts.UI.ShopCustomizations
{
    public class ShopAvatarsManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _avatarPrefab;

        [SerializeField]
        private GameObject _shopContent;

        private IAvatarsService _avatarsService;
        private IImagesService _imagesService;

        [Inject]
        public void Construct(
            IAvatarsService avatarsService,
            IImagesService imagesService)
        {
            _avatarsService = avatarsService;
            _imagesService = imagesService;
        }

        private async void Awake()
        {
            await InitializeShopAsync();
        }

        private async UniTask InitializeShopAsync()
        {
            await TaskRunner.RunWithGlobalErrorHandling(async () =>
            {
                var request = new EmptyRequest();

                var avatars = await _avatarsService.GetAvailable(request);
                await InstantiateAvatars(avatars);
            });
        }

        private async UniTask InstantiateAvatars(List<GetAvailableAvatarsResponse> avatars)
        {
            foreach (var avatar in avatars)
            {
                await TaskRunner.RunWithGlobalErrorHandling(async () =>
                {
                    var request = new LoadImageRequest(
                        avatar.FileName,
                        avatar.S3Path);

                    var image = await _imagesService.LoadImage(request);

                    var avatarModel = new Avatar(avatar.Id, avatar.S3Path, avatar.Name, avatar.Description, avatar.FileName);

                    var shopAvatar = Instantiate(_avatarPrefab, _shopContent.transform);
                    var avatarController = shopAvatar.GetComponent<ShopAvatarsController>();

                    if (avatarController != null)
                    {
                        avatarController.SetData(
                            image,
                            avatarModel,
                            avatar.NecessaryMmr,
                            avatar.NecessaryGames,
                            avatar.NecessaryWins);
                    }
                });
            }
        }
    }
}