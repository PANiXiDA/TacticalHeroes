using Assets.Scripts.Common.Enumerations;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class ButtonView : MonoBehaviour
    {
        [SerializeField] private BattleButtonType _type;
        [SerializeField] private Image _icon;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;

        public BattleButtonType GetButtonType() => _type;
        public void SetActive(bool isActive) => _icon.sprite = isActive ? _activeSprite : _inactiveSprite;
    }
}
