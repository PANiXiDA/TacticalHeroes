using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ProfileManager : MonoBehaviour
{
    [SerializeField]
    private Image _playerFrame, _playerAvatar, _leagueIcon;

    [SerializeField]
    private TextMeshProUGUI _games, _wins, _loses, _league, _mmr;

    public PlayerProfile playerProfile;

    private const string _loginUrl = "auth/login";
    private const string _authScene = "Auth";

    private async void Awake()
    {
        var email = PlayerPrefs.GetString("Email");
        var password = PlayerPrefs.GetString("Password");
        var login = new LoginRequest(email, password);

        try
        {
            var result = await UniversalWebRequest.SendRequest<LoginRequest, PlayerProfile>(_loginUrl, RequestType.POST, login);

            if (result != null && result.IsSuccess)
            {
                playerProfile = result.Payload;

                _games.text = playerProfile.Games.ToString();
                _wins.text = playerProfile.Wins.ToString();
                _loses.text = playerProfile.Loses.ToString();
                _mmr.text = playerProfile.Mmr.ToString();

                var league = DetermineLeague(playerProfile.Mmr);
                _league.text = league.ToString();
                _league.color = GetLeagueColor(league);
                await LoadLeagueIconAsync(league);
            }
            else
            {               
                Debug.LogError($"Unexpected error: {result.ToString()}");
                SceneManager.LoadScene(_authScene);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error: {ex.Message}");
            SceneManager.LoadScene(_authScene);
        }

        //string photoUrl = PlayerPrefs.GetString("ImageUrl");

        //if (!string.IsNullOrEmpty(photoUrl))
        //{
        //    await SetProfileImage(photoUrl);
        //}
    }

    private League DetermineLeague(int mmr)
    {
        if (mmr < 1500)
            return League.Bronze;
        else if (mmr < 2000)
            return League.Silver;
        else if (mmr < 2500)
            return League.Gold;
        else if (mmr < 3000)
            return League.Platinum;
        else if (mmr < 3500)
            return League.Diamond;
        else if (mmr < 4000)
            return League.Master;
        else
            return League.GrandMaster;
    }

    private Color GetLeagueColor(League league)
    {
        switch (league)
        {
            case League.Bronze:
                return new Color(0.804f, 0.498f, 0.196f);
            case League.Silver:
                return new Color(0.752f, 0.752f, 0.752f);
            case League.Gold:
                return new Color(1.0f, 0.843f, 0.0f);
            case League.Platinum:
                return new Color(0.678f, 0.847f, 0.902f);
            case League.Diamond:
                return new Color(0.0f, 0.749f, 1.0f);
            case League.Master:
                return Color.black;
            case League.GrandMaster:
                return new Color(0.8f, 0.0f, 0.7f);
            default:
                return Color.white;
        }
    }

    private async UniTask LoadLeagueIconAsync(League league)
    {
        string assetAddress = $"LeagueIcons/{league}";

        var handle = Addressables.LoadAssetAsync<Sprite>(assetAddress);
        var sprite = await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _leagueIcon.sprite = handle.Result;
        }
        else
        {
            Debug.LogWarning($"Failed to load league icon for {league} with address: {assetAddress}");
        }

        Addressables.Release(handle);
    }

    //private async UniTask SetProfileImage(string photoUrl)
    //{
    //if (imageCache.ContainsKey(photoUrl))
    //{
    //    _playerImage.sprite = imageCache[photoUrl];
    //}
    //else
    //{
    //    Sprite sprite = await GetImageFromUrl(photoUrl);

    //    if (sprite != null)
    //    {
    //        _playerImage.sprite = sprite;
    //    }
    //}
    //}

    public async UniTask<Sprite> GetImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = SpriteFromTexture2D(texture);

            //imageCache[url] = sprite;

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
