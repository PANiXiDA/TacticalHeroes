using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Common.WebRequest.JWT;
using Assets.Scripts.Infrastructure.Requests.AuthService;
using Assets.Scripts.Services.Interfaces;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.UI.Profile
{
    public class ProfileSceneManager : BaseProfileScene
    {
        [SerializeField]
        private TextMeshProUGUI _nickname, _lastLogin;

        [SerializeField]
        private GameObject _statisticContent, _statisticPrefab;

        private IAuthService _authService;

        [Inject]
        public void Construct(
            IAuthService authService)
        {
            _authService = authService;
        }

        private async void Start()
        {
            _playerProfile = PlayerProfile.Instance;
            InitializeSceneAsync();
            await InitializePlayerProfileAsync();
        }

        private void InitializeSceneAsync()
        {
            _playerFrame.SetActive(true);
            _playerAvatar.SetActive(true);

            _nickname.text = _playerProfile.Nickname;

            UpdateLastLoginUI(_lastLogin, Helpers.ConvertUtcToLocalTime(_playerProfile.LastLogin));
            AddStatisticRow();
        }

        public async void Logout()
        {
            var refreshToken = JwtTokenManager.LoadRefreshToken();
            await _authService.Logout(new LogoutRequest(refreshToken));
            SceneManager.LoadScene(SceneConstants.MenuScene);
        }

        private void UpdateLastLoginUI(TextMeshProUGUI lastLoginText, DateTime lastLogin)
        {
            if (lastLogin.AddMinutes(30) <= DateTime.UtcNow)
            {
                lastLoginText.color = Color.white;
                lastLoginText.text = lastLogin.ToString("dd.MM.yyyy HH:mm");
            }
            else
            {
                lastLoginText.color = Color.green;
                lastLoginText.text = "Online";
            }
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
    }
}
