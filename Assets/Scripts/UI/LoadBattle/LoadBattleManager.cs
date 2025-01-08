using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Requests.ImageService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.LoadBattle
{
    public class LoadBattleManager : MonoBehaviour
    {
        [SerializeField]
        private MatchSettings _matchSettings;

        [SerializeField]
        private TextMeshProUGUI _battleType;

        [SerializeField]
        private TextMeshProUGUI _mapName;

        [SerializeField]
        private Image _mapImage;

        [SerializeField]
        private TextMeshProUGUI _loadingText;

        [SerializeField]
        private Slider _loadingBar;

        [SerializeField]
        private TextMeshProUGUI _suggestionsText;

        /// <summary>
        /// player
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _playerNickname;

        [SerializeField]
        private Image _playerFrame;

        [SerializeField]
        private Image _playerAvatar;

        [SerializeField]
        private Image _playerLeagueIcon;

        [SerializeField]
        private TextMeshProUGUI _playerMmr;

        [SerializeField]
        private Image _playerFactionIcon;

        /// <summary>
        /// opponent
        /// </summary>

        [SerializeField]
        private TextMeshProUGUI _opponentNickname;

        [SerializeField]
        private Image _opponentFrame;

        [SerializeField]
        private Image _opponentAvatar;

        [SerializeField]
        private Image _opponentLeagueIcon;

        [SerializeField]
        private TextMeshProUGUI _opponentMmr;

        [SerializeField]
        private Image _opponentFactionIcon;

        private AsyncOperationHandle<Sprite> _loadedSpriteHandle;

        protected PlayerProfile _playerProfile;
        protected IImagesService _imagesService;

        [Inject]
        public void Construct(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        private void Start()
        {
            _playerProfile = PlayerProfile.Instance;
            InitializeBattleSettings().Forget();
            InitializePlayerPanel().Forget();
            InitializeOpponentPanel().Forget();
        }

        public async UniTaskVoid InitializeBattleSettings()
        {
            _battleType.text = "Duel 1x1";
            _mapName.text = "Wasteland";
            _suggestionsText.text = BattleSuggestionsConstants.GetRandomSuggestion();
            //_mapImage.sprite = _mapImage.sprite;

            for (float progress = 0; progress <= 1; progress += 0.01f)
            {
                _loadingBar.value = progress;
                _loadingText.text = $"{Mathf.RoundToInt(progress * 100)}%";
                await UniTask.Delay(100);
            }

            SceneManagerHelper.Instance.ChangeScene(SceneConstants.BattleScene);
        }

        public async UniTask InitializePlayerPanel()
        {
            _playerNickname.text = _playerProfile.Nickname;
            await LoadPlayerImageAsync(_playerProfile.Frame.FileName, _playerProfile.Frame.S3Path, _playerFrame);
            await LoadPlayerImageAsync(_playerProfile.Avatar.FileName, _playerProfile.Avatar.S3Path, _playerAvatar);
            _playerMmr.text = _playerProfile.Mmr.ToString();
            var league = DetermineLeague(_playerProfile.Mmr);
            await LoadLeagueIconAsync(league, _playerLeagueIcon);
            //_playerFactionIcon.sprite = _playerFactionIcon.sprite;
        }

        public async UniTask InitializeOpponentPanel()
        {
            _opponentNickname.text = _matchSettings.OpponentNickname;
            await LoadPlayerImageAsync(_matchSettings.Frame.FileName, _matchSettings.Frame.S3Path, _opponentFrame);
            await LoadPlayerImageAsync(_matchSettings.Avatar.FileName, _matchSettings.Avatar.S3Path, _opponentAvatar);
            _opponentMmr.text = _matchSettings.Mmr.ToString();
            var league = DetermineLeague(_matchSettings.Mmr);
            await LoadLeagueIconAsync(league, _opponentLeagueIcon);
            //_opponentFactionIcon.sprite = _opponentFactionIcon.sprite;
        }

        private async UniTask LoadPlayerImageAsync(string fileName, string s3Path, Image image)
        {
            await TaskRunner.RunWithGlobalErrorHandling(async () =>
            {
                var request = new LoadImageRequest(fileName, s3Path);
                var sprite = await _imagesService.LoadImage(request);

                image.sprite = sprite;
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

        private async UniTask LoadLeagueIconAsync(League league, Image _leagueIcon)
        {
            string assetAddress = $"LeagueIcons/{league}";

            _loadedSpriteHandle = Addressables.LoadAssetAsync<Sprite>(assetAddress);

            try
            {
                var sprite = await _loadedSpriteHandle.ToUniTask();

                if (_loadedSpriteHandle.Status == AsyncOperationStatus.Succeeded && _leagueIcon != null)
                {
                    _leagueIcon.sprite = sprite;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Icon loading canceled.");
            }
        }
    }
}