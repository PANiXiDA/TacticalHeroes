using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Infrastructure.Requests;
using Assets.Scripts.Infrastructure.Requests.ImageService;
using Assets.Scripts.Infrastructure.Responses.FramesService;
using Assets.Scripts.Services.Interfaces;
using Assets.Scripts.UI.ShopCustomizations.Extensions;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.UI.ShopCustomizations
{
    public class ShopFramesManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _framePrefab;

        [SerializeField]
        private GameObject _shopContent;

        private IFramesService _framesService;
        private IImagesService _imagesService;

        [Inject]
        public void Construct(
            IFramesService framesService,
            IImagesService imagesService)
        {
            _framesService = framesService;
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

                var avatars = await _framesService.GetAvailable(request);
                await InstantiateFrames(avatars);
            });
        }

        private async UniTask InstantiateFrames(List<GetAvailableFramesResponse> frames)
        {
            foreach (var frame in frames)
            {
                await TaskRunner.RunWithGlobalErrorHandling(async () =>
                {
                    var request = new LoadImageRequest(
                        frame.FileName,
                        frame.S3Path);

                    var image = await _imagesService.LoadImage(request);

                    var frameModel = new Frame(frame.Id, frame.S3Path, frame.Name, frame.Description, frame.FileName);

                    var shopAvatar = Instantiate(_framePrefab, _shopContent.transform);
                    var frameController = shopAvatar.GetComponent<ShopFramesController>();

                    if (frameController != null)
                    {
                        frameController.SetData(
                            image,
                            frameModel,
                            frame.NecessaryMmr,
                            frame.NecessaryGames,
                            frame.NecessaryWins);
                    }
                });
            }
        }
    }
}
