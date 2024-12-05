using Assets.Scripts.Common.Constants;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Services.Implementations
{
    public class ImagesService : IImagesService
    {
        public ImagesService() { }

        public async UniTask<Sprite> LoadImage(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);

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
    }
}
