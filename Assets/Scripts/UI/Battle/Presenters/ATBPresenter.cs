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

        private readonly List<ATBItemView> _atb = new();

        [Inject] private readonly DiContainer _container;
        [Inject] private readonly IATBService _atbService;
        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IPlayerColorsService _playerColorsService;
        [Inject] private readonly IAttacksService _attacksService;

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
                .Subscribe(currentActiveGameObject => RemoveATBItem(currentActiveGameObject.Id))
                .AddTo(_disposables);

            _attacksService.OnAttackDone
                .Subscribe(attackEvent => AttackDone(attackEvent))
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

        private void AttackDone(AttackEvent attackEvent)
        {
            if (attackEvent.Defender.Count > 0)
            {
                UpdateUnitCountInATB(attackEvent.Defender.Id, attackEvent.Defender.Count);
            }
            else
            {
                RemoveUnitFromATB(attackEvent.Defender.Id);
            }
        }

        private void UpdateUnitCountInATB(Guid unitId, int newCount)
        {
            _atb.Where(view => view.Data.Id == unitId)
                .ToList()
                .ForEach(view => view.UpdateCount(newCount));
        }

        private void RemoveATBItem(Guid atbItemId)
        {
            var view = _atb.FirstOrDefault(atbItem => atbItem.Data.Id == atbItemId);
            _atb.Remove(view);
            Destroy(view.gameObject);
        }

        private void RemoveUnitFromATB(Guid unitId)
        {
            _atb.RemoveAll(view =>
            {
                if (view.Data.Id == unitId)
                {
                    Destroy(view.gameObject);
                    return true;
                }
                return false;
            });
        }
    }
}
