using Assets.Scripts.Domain.DTO.Models;
using Cysharp.Threading.Tasks;

using TMPro;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

using Zenject;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class ATBItemView : MonoBehaviour
    {
        private const string AddressablePrefix = "Sprites/Avatars";

        [SerializeField] private Image _frame;
        [SerializeField] private Image _avatar;
        [SerializeField] private TextMeshProUGUI _countText;

        [Inject] public ATBItem Data { get; private set; }
        [Inject] public Color FrameColor { get; private set; }

        private void OnEnable()
        {
            LoadAsync().Forget();
        }

        private void OnDestroy() => Addressables.Release(_avatar.sprite);

        public void UpdateCount(int newCount)
        {
            Data.Count = newCount;
            _countText.text = Data.Count.ToString();
        }

        private async UniTaskVoid LoadAsync()
        {
            _frame.color = FrameColor;

            var sprite = await Addressables.LoadAssetAsync<Sprite>($"{AddressablePrefix}/{Data.Name}");
            _avatar.sprite = sprite;

            if (Data.Count.HasValue)
            {
                _countText.text = Data.Count.Value.ToString();
            }
        }
    }
}
