using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.WebRequest;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading;
using UnityEngine.UI;
using System.Linq;

public class ProfileManager : MonoBehaviour
{
    private CancellationTokenSource _cts;

    [SerializeField]
    private GameObject _playerFrame, _playerAvatar, _leagueIcon;

    [SerializeField]
    private TextMeshProUGUI _games, _wins, _loses, _league, _mmr;

    public PlayerProfile playerProfile;

    private const string _playerUrl = "players";
    private const string _authScene = "Auth";

    private void Awake()
    {
        _cts = new CancellationTokenSource();
        InitializeProfileAsync(_cts.Token).Forget();
    }

    private void OnDestroy()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    private async UniTaskVoid InitializeProfileAsync(CancellationToken token)
    {
        _leagueIcon.SetActive(true);
        _playerFrame.SetActive(true);
        _playerAvatar.SetActive(true);

        try
        {
            var result = await UniversalWebRequest.SendRequest<object, PlayerProfile>(_playerUrl, RequestType.GET, null);

            if (result != null && result.IsSuccess)
            {
                playerProfile = result.Payload;

                if (token.IsCancellationRequested) return;

                _games.text = playerProfile.Games.ToString();
                _wins.text = playerProfile.Wins.ToString();
                _loses.text = playerProfile.Loses.ToString();
                _mmr.text = playerProfile.Mmr.ToString();

                var league = DetermineLeague(playerProfile.Mmr);

                if (token.IsCancellationRequested) return;

                _league.text = league.ToString();
                _league.color = GetLeagueColor(league);
                await LoadLeagueIconAsync(league, token);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error: {ex.Message}");
        }
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

    private async UniTask LoadLeagueIconAsync(League league, CancellationToken token)
    {
        string assetAddress = $"LeagueIcons/{league}";

        var handle = Addressables.LoadAssetAsync<Sprite>(assetAddress);

        try
        {
            var sprite = await handle.ToUniTask(cancellationToken: token);

            if (handle.Status == AsyncOperationStatus.Succeeded && _leagueIcon != null && !token.IsCancellationRequested)
            {
                _leagueIcon.GetComponentInChildren<Image>().sprite = handle.Result;
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Icon loading canceled.");
        }
        finally
        {
            Addressables.Release(handle);
        }
    }
}
