using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.Helpers;
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

        [Inject] private readonly DiContainer _container;
        [Inject] private readonly IATBService _atbService;
        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IPlayerColorsService _playerColorsService;

        private void OnEnable()
        {
            BindStreams();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void BindStreams()
        {
            _atbService.OnTurnOrderGenerated
                .Subscribe(turnOrder => SetAtb(turnOrder))
                .AddTo(_disposables);

            _battleTurnsService.OnTurnEnded
                .Subscribe(id => RemoveAtbItem(id))
                .AddTo(_disposables);
        }

        private void SetAtb(IReadOnlyList<ATBItem> items)
        {
            foreach (var item in items)
            {
                var color = _playerColorsService.GetRgb24(item.PlayerId).ToColor();
                var view = _container.InstantiatePrefabForComponent<ATBItemView>(_atbItemPrefab, _atbContainer, new object[] { item, color });
                _atb.Add(view);
            }
        }

        private void RemoveAtbItem(Guid atbItemId)
        {
            var view = _atb.FirstOrDefault(atbItem => atbItem.Data.Id == atbItemId);
            _atb.Remove(view);
            Destroy(view.gameObject);
        }
    }
}
