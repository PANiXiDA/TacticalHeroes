using Assets.Scripts.Common.Constants;
using Assets.Scripts.Infrastructure.Requests.ImageService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Services.Implementations
{
    public class ImagesService : IImagesService
    {
        private readonly string _localCachePath;

        public ImagesService()
        {
            _localCachePath = Path.Combine(Application.persistentDataPath, "ImageCache");
            if (!Directory.Exists(_localCachePath))
            {
                Directory.CreateDirectory(_localCachePath);
            }
        }

        public async UniTask<Sprite> LoadImage(LoadImageRequest loadImageRequest)
        {
            string filePath = Path.Combine(_localCachePath, loadImageRequest.FileName);

            if (File.Exists(filePath))
            {
                return LoadSpriteFromFile(filePath);
            }

            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(loadImageRequest.Url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    SaveTextureToFile(filePath, texture);

                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    return sprite;
                }
                else
                {
                    throw new ApplicationException(ErrorConstants.ServerUnavailable);
                }
            }
        }

        private Sprite LoadSpriteFromFile(string filePath)
        {
            byte[] imageData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        private void SaveTextureToFile(string filePath, Texture2D texture)
        {
            byte[] imageData = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, imageData);
        }
    }
}
