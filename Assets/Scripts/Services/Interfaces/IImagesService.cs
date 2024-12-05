using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Services.Interfaces
{
    public interface IImagesService
    {
        UniTask<Sprite> LoadImage(string url);
    }
}
