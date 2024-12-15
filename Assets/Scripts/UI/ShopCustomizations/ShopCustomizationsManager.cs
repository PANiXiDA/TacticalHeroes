using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Helpers;
using Assets.Scripts.Infrastructure.Models;
using Assets.Scripts.Infrastructure.Requests.PlayersService;
using Assets.Scripts.Services.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Avatar = Assets.Scripts.Infrastructure.Models.Avatar;

namespace Assets.Scripts.UI.ShopCustomizations
{
    public class ShopCustomizationsManager : BaseProfileScene
    {
        [SerializeField]
        private GameObject _shopAvatars, _frameAvatars;

        [SerializeField]
        private TextMeshProUGUI _nickname, _gold;

        [SerializeField]
        private Image _premiumIcon;

        [SerializeField]
        private GameObject _shopAvatarsBtn, _shopFramesBtn;

        public static ShopCustomizationsManager Instance { get; private set; }
        private PopupManager _popupManager;
        private SceneManagerHelper _sceneManagerHelper;

        private Avatar _avatar;
        private Frame _frame;

        private IPlayersService _playersService;

        [Inject]
        public void Construct(
            IPlayersService playersService)
        {
            _playersService = playersService;
        }

        private async void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _playerProfile = PlayerProfile.Instance;
            _popupManager = PopupManager.Instance;
            _sceneManagerHelper = SceneManagerHelper.Instance;

            InitializeSceneAsync();
            await InitializePlayerProfileAsync();
        }

        private void InitializeSceneAsync()
        {
            _playerFrame.SetActive(true);
            _playerAvatar.SetActive(true);

            if (_playerProfile.Premium)
            {
                ImageHelper.SetActiveIcon(_premiumIcon);
            }
            else
            {
                ImageHelper.SetInactiveIcon(_premiumIcon);
            }

            _nickname.text = _playerProfile.Nickname;
            _gold.text = _playerProfile.Gold.ToString();
        }

        public void UpdatePlayerAvatar(Avatar avatar, Sprite avatarSprite)
        {
            _playerAvatar.GetComponentInChildren<Image>().sprite = avatarSprite;
            _avatar = avatar;
        }

        public void UpdatePlayerFrame(Frame frame, Sprite frameSprite)
        {
            _playerFrame.GetComponentInChildren<Image>().sprite = frameSprite;
            _frame = frame;
        }

        public void ShowShopAvatars()
        {
            _shopAvatars.SetActive(true);
            var shopAvatarsBtn = _shopAvatarsBtn.GetComponent<Image>();
            ImageHelper.SetActiveIcon(shopAvatarsBtn);

            _frameAvatars.SetActive(false);
            var shopFramesBtn = _shopFramesBtn.GetComponent<Image>();
            ImageHelper.SetInactiveIcon(shopFramesBtn);
        }

        public void ShowFrameAvatars()
        {
            _shopAvatars.SetActive(false);
            var shopAvatarsBtn = _shopAvatarsBtn.GetComponent<Image>();
            ImageHelper.SetInactiveIcon(shopAvatarsBtn);

            _frameAvatars.SetActive(true);
            var shopFramesBtn = _shopFramesBtn.GetComponent<Image>();
            ImageHelper.SetActiveIcon(shopFramesBtn);
        }

        public void SavePlayerShopItems()
        {
            if (_frame != null)
            {
                TaskRunner.RunWithGlobalErrorHandling(async () =>
                {
                    var request = new UpdateFrameRequest(_frame.Id);
                    var response = await _playersService.UpdatePlayerFrame(request);
                    _popupManager.ShowPopup(SuccessConstants.Success);

                    _playerProfile.Frame = _frame;
                });
            }
            if (_avatar != null)
            {
                TaskRunner.RunWithGlobalErrorHandling(async () =>
                {
                    var request = new UpdateAvatarRequest(_avatar.Id);
                    var response = await _playersService.UpdatePlayerAvatar(request);
                    _popupManager.ShowPopup(SuccessConstants.Success);

                    _playerProfile.Avatar = _avatar;
                });
            }
        }

        public void ReturnBtn()
        {
            if (!string.IsNullOrEmpty(_sceneManagerHelper.PreviousScene))
            {
                SceneManager.LoadScene(_sceneManagerHelper.PreviousScene);
            }
        }
    }
}
