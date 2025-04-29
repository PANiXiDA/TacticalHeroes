using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using R3;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(UnitView))]
    public sealed class UnitsPresenter : MonoBehaviour
    {
        private UnitView _view;

        private bool _isActive;

        [Inject] private GridsPresenter _grid;

        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IMovementsService _movementsService;

        private readonly CompositeDisposable _disposables = new();

        private void OnEnable()
        {
            CacheComponents();
            BindStreams();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void CacheComponents()
        {
            _view = GetComponent<UnitView>();
        }

        private void BindStreams()
        {
            _battleTurnsService.OnTurnStarted
                .Subscribe(id =>
                {
                    _isActive = _view.Data.Id == id;
                })
                .AddTo(_disposables);

            _movementsService.OnPathComputed
                .Where(_ => _isActive)
                .Subscribe(path => Move(path))
                .AddTo(_disposables);
        }

        private void Move(IReadOnlyList<Tile> tiles)
        {
            Vector3[] path = tiles.Select(tile => _grid.GetTile(tile.X, tile.Y).transform.position).ToArray();
            _view.PlayMoveAnimation();
            transform.DOPath(path, 1, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear);
        }
    }
}
