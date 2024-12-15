using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Infrastructure.Models;

namespace Assets.Scripts.UI.ShopCustomizations.Extensions
{
    public class ShopFramesController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Image _frameImage;

        [SerializeField]
        private TextMeshProUGUI _title, _description;

        [SerializeField]
        private TextMeshProUGUI _mmr, _games, _wins;

        private ShopCustomizationsManager _shopCustomizationsManager;
        private PlayerProfile _playerProfile;

        private Frame _frame;

        private void Awake()
        {
            _shopCustomizationsManager = ShopCustomizationsManager.Instance;
            _playerProfile = PlayerProfile.Instance;
        }

        private bool IsLocked { get; set; }

        public void SetData(
            Sprite image,
            Frame frame,
            int mmr,
            int games,
            int wins)
        {
            _frameImage.color = CalculateFrameTransparency(_playerProfile, mmr, games, wins);
            _frameImage.sprite = image;

            _title.text = frame.Name;
            _description.text = frame.Description;

            _frame = frame;

            SetTextWithColor(_mmr, mmr, _playerProfile.Mmr);
            SetTextWithColor(_games, games, _playerProfile.Games);
            SetTextWithColor(_wins, wins, _playerProfile.Wins);
        }

        private Color CalculateFrameTransparency(PlayerProfile playerProfile, int mmr, int games, int wins)
        {
            Color color = _frameImage.color;
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
                _shopCustomizationsManager.UpdatePlayerFrame(_frame, _frameImage.sprite);
            }
        }
    }
}
