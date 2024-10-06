using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField]
    private Image _playerImage;

    public static ProfileManager Instance { get; private set; }

    public static Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();

    private async void Awake()
    {
        Instance = this;

        string photoUrl = PlayerPrefs.GetString("ImageUrl");

        if (!string.IsNullOrEmpty(photoUrl))
        {
            await SetProfileImage(photoUrl);
        }

        DontDestroyOnLoad(gameObject);
    }

    private async UniTask SetProfileImage(string photoUrl)
    {
        if (imageCache.ContainsKey(photoUrl))
        {
            _playerImage.sprite = imageCache[photoUrl];
        }
        else
        {
            Sprite sprite = await GetImageFromUrl(photoUrl);

            if (sprite != null)
            {
                _playerImage.sprite = sprite;
            }
        }
    }

    public async UniTask<Sprite> GetImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = SpriteFromTexture2D(texture);

            imageCache[url] = sprite;

            return sprite;
        }
        else
        {
            Debug.LogError($"Ошибка загрузки изображения: {request.error}");

            return null;
        }

    }

    private Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
