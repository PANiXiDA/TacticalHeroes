using System.Collections.Generic;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;

using Cysharp.Threading.Tasks;

using R3;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    public sealed class ATBPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform _atbContainer;
        [SerializeField] private ATBItemView _atbItemPrefab;

        private readonly CompositeDisposable _disposables = new();
        private readonly Color[] _palette =
        {
            Color.red,
            Color.blue,
            Color.yellow,
            Color.green,
        };

        private readonly List<ATBItemView> _atb = new();
        private readonly Dictionary<int, Color> _colorsByPlayer = new();

        [Inject] private readonly IATBService _atbService;

        private void OnEnable()
        {
            _atbService.OnTurnOrderGenerated
                .Subscribe(turnOrder => SetAtb(turnOrder).Forget())
                .AddTo(_disposables);
        }

        private void OnDestroy() => _disposables.Dispose();

        private async UniTask SetAtb(IReadOnlyList<ATBItem> items)
        {
            foreach (var item in items)
            {
                var color = GetColorForPlayer(item.PlayerId);

                var view = Instantiate(_atbItemPrefab, _atbContainer);
                await view.Init(item, color);

                _atb.Add(view);
            }
        }

        private Color GetColorForPlayer(int playerId)
        {
            if (_colorsByPlayer.TryGetValue(playerId, out var color))
            {
                return color;
            }

            int index = _colorsByPlayer.Count % _palette.Length;
            color = _palette[index];
            _colorsByPlayer[playerId] = color;
            return color;
        }
    }
}
