using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.GameEngine.DTO.Enums;
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
        private const float MoveSpeed = 5f;

        private UnitView _view;

        [Inject] private GridsPresenter _grid;

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
            _movementsService.OnPathComputed
                .Where(path => path.UnitId == _view.Data.Id)
                .Subscribe(path => Move(path.Tiles))
                .AddTo(_disposables);
        }

        private void Move(IReadOnlyList<Tile> tiles)
        {
            Vector3[] path = tiles
                .Select(tile => _grid.GetTile(tile.X, tile.Y).transform.position)
                .ToArray();

            bool defaultFlip = _view.Side == PlayerSide.Right;
            float deltaX = path.Last().x - path.First().x;
            if (_view.Side == PlayerSide.Right && deltaX > 0
                || _view.Side == PlayerSide.Left && deltaX < 0)
            {
                _view.Flip(!defaultFlip);
            }

            _view.SetMovingState(true);
            _view.IncrementSortingOrder();

            float totalDistance = 0f;
            for (int i = 1; i < path.Length; i++)
            {
                totalDistance += Vector3.Distance(path[i - 1], path[i]);
            }
            float duration = totalDistance / MoveSpeed;

            transform
                .DOPath(path, duration, PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _view.SetMovingState(false);
                    _view.Flip(defaultFlip);
                    _view.DecrementSortingOrder();
                });
        }
    }
}
