using Assets.Scripts.Domain.DTO.Models;
using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class ATBItemView : MonoBehaviour
    {
        private const string AddressablePrefix = "Sprites/Avatars";

        [SerializeField] private Image _frame;
        [SerializeField] private Image _avatar;
        [SerializeField] private TextMeshProUGUI _countText;

        public async UniTask Init(ATBItem atbItem, Color color)
        {
            _frame.color = color;
            _avatar.sprite = await Addressables.LoadAssetAsync<Sprite>($"{AddressablePrefix}/{atbItem.Name}");

            if (atbItem.Count.HasValue)
            {
                _countText.text = atbItem.Count.Value.ToString();
            }
        }
    }

}
