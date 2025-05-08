using Assets.Scripts.Infrastructure.Requests.ImageService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IImagesService
    {
        UniTask<Sprite> LoadImage(LoadImageRequest loadImageRequest);
    }
}
