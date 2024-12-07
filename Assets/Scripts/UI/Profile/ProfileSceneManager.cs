using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Common.WebRequest.JWT;
using Assets.Scripts.Infrastructure.Requests.AuthService;
using Assets.Scripts.Services.Interfaces;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Profile
{
    public class ProfileSceneManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerFrame, _playerAvatar;

        [SerializeField]
        private TextMeshProUGUI _nickname, _lastLogin;

        [SerializeField]
        private GameObject _statisticContent, _statisticPrefab;

        private PlayerProfile _playerProfile;

        private IAuthService _authService;
        private IImagesService _imagesService;

        [Inject]
        public void Construct(
            IAuthService authService,
            IImagesService imagesService)
        {
            _authService = authService;
            _imagesService = imagesService;
        }

        private async void Start()
        {
            _playerProfile = PlayerProfile.Instance;
            await InitializeSceneAsync();
        }

        private async UniTask InitializeSceneAsync()
        {
            _playerFrame.SetActive(true);
            _playerAvatar.SetActive(true);

            _nickname.text = _playerProfile.Nickname;
            _lastLogin.text = _playerProfile.LastLogin.ToString();

            AddStatisticRow();

            await TaskRunner.RunWithGlobalErrorHandling(async () =>
            {
                var playerAvatar = await _imagesService.LoadImage(_playerProfile.Avatar.S3Path);
                if (_playerAvatar != null)
                {
                    _playerAvatar.GetComponentInChildren<Image>().sprite = playerAvatar;
                }
            });
        }

        private void AddStatisticRow()
        {
            var newStatisticRow = Instantiate(_statisticPrefab, _statisticContent.transform);

            var texts = newStatisticRow.GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = "Total";
            texts[1].text = _playerProfile.Mmr.ToString();
            texts[2].text = _playerProfile.Wins.ToString();
            texts[3].text = _playerProfile.Loses.ToString();
            if (_playerProfile.Games > 0)
            {
                texts[4].text = $"{((float)_playerProfile.Wins / _playerProfile.Games):P0}";
            }
            else
            {
                texts[4].text = "0%";
            }
        }

        public async void Logout()
        {
            var refreshToken = JwtTokenManager.LoadRefreshToken();
            await _authService.Logout(new LogoutRequest(refreshToken));
            SceneManager.LoadScene(SceneConstants.MenuScene);
        }
    }
}
