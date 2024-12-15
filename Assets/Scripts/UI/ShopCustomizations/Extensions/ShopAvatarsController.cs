using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Avatar = Assets.Scripts.Infrastructure.Models.Avatar;

namespace Assets.Scripts.UI.ShopCustomizations.Extensions
{
    public class ShopAvatarsController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private TextMeshProUGUI _title, _description;

        [SerializeField]
        private TextMeshProUGUI _mmr, _games, _wins;

        private ShopCustomizationsManager _shopCustomizationsManager;
        private PlayerProfile _playerProfile;

        private Avatar _avatar;

        private void Awake()
        {
            _shopCustomizationsManager = ShopCustomizationsManager.Instance;
            _playerProfile = PlayerProfile.Instance;
        }

        private bool IsLocked { get; set; }

        public void SetData(
            Sprite image,
            Avatar avatar,
            int mmr,
            int games,
            int wins)
        {
            _avatarImage.color = CalculateAvatarTransparency(_playerProfile, mmr, games, wins);
            _avatarImage.sprite = image;

            _title.text = avatar.Name;
            _description.text = avatar.Description;

            _avatar = avatar;

            SetTextWithColor(_mmr, mmr, _playerProfile.Mmr);
            SetTextWithColor(_games, games, _playerProfile.Games);
            SetTextWithColor(_wins, wins, _playerProfile.Wins);
        }

        private Color CalculateAvatarTransparency(PlayerProfile playerProfile, int mmr, int games, int wins)
        {
            Color color = _avatarImage.color;
            bool isLocked = playerProfile.Mmr < mmr || playerProfile.Games < games || playerProfile.Wins < wins;
            color.a = isLocked ? 100f / 255f : 1.0f;

            IsLocked = isLocked;

            return color;
        }

        private void SetTextWithColor(TextMeshProUGUI textComponent, int requiredValue, int playerValue)
        {
            textComponent.color = playerValue < requiredValue ? Color.red : Color.green;
            textComponent.text = requiredValue.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsLocked)
            {
                _shopCustomizationsManager.UpdatePlayerAvatar(_avatar, _avatarImage.sprite);
            }
        }
    }
}
