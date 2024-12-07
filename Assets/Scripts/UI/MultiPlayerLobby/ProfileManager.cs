using Assets.Scripts.Common.Enumerations;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Assets.Scripts.Services.Interfaces;
using Zenject;
using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Requests;

namespace Assets.Scripts.UI.MultiPlayerLobby
{
    public class ProfileManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerFrame, _playerAvatar, _leagueIcon;

        [SerializeField]
        private TextMeshProUGUI _games, _wins, _loses, _league, _mmr;

        private AsyncOperationHandle<Sprite> _loadedSpriteHandle;
        private PlayerProfile _playerProfile;

        private IPlayersService _playerService;
        private IImagesService _imagesService;

        [Inject]
        public void Construct(
            IPlayersService playerService,
            IImagesService imagesService)
        {
            _playerService = playerService;
            _imagesService = imagesService;
        }

        private async void Start()
        {
            _playerProfile = PlayerProfile.Instance;
            await InitializeProfileAsync();
        }

        private void OnDestroy()
        {
            if (_loadedSpriteHandle.IsValid())
            {
                Addressables.Release(_loadedSpriteHandle);
            }
        }

        private async UniTask InitializeProfileAsync()
        {
            var request = new EmptyRequest();

            await TaskRunner.RunWithGlobalErrorHandling(async () =>
            {
                var response = await _playerService.GetPlayerProfile(request);

                _playerProfile.Initialize(
                    response.Nickname,
                    response.Games,
                    response.Wins,
                    response.Loses,
                    response.Mmr,
                    response.LastLogin,
                    response.Avatar);
            });

            _leagueIcon.SetActive(true);
            _playerFrame.SetActive(true);
            _playerAvatar.SetActive(true);

            _games.text = _playerProfile.Games.ToString();
            _wins.text = _playerProfile.Wins.ToString();
            _loses.text = _playerProfile.Loses.ToString();
            _mmr.text = _playerProfile.Mmr.ToString();

            var league = DetermineLeague(_playerProfile.Mmr);
            _league.text = league.ToString();
            _league.color = GetLeagueColor(league);
            await LoadLeagueIconAsync(league);

            await TaskRunner.RunWithGlobalErrorHandling(async () =>
            {
                var playerAvatar = await _imagesService.LoadImage(_playerProfile.Avatar.S3Path);
                if (_playerAvatar != null)
                {
                    _playerAvatar.GetComponentInChildren<Image>().sprite = playerAvatar;
                }
            });
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

            _loadedSpriteHandle = Addressables.LoadAssetAsync<Sprite>(assetAddress);

            try
            {
                var sprite = await _loadedSpriteHandle.ToUniTask();

                if (_loadedSpriteHandle.Status == AsyncOperationStatus.Succeeded && _leagueIcon != null)
                {
                    _leagueIcon.GetComponentInChildren<Image>().sprite = sprite;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Icon loading canceled.");
            }
        }
    }
}