using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.Helpers;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;

using R3;

using UnityEngine;

using Zenject;

using DomainGameObject = Assets.Scripts.GameEngine.Domain.Core.GameObject;
using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.UI.Battle.Presenters
{
    public sealed class AttackPreviewsPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _swordPrefab;
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private GameObject _brokenArrowPrefab;

        [Inject] private GridsPresenter _grid;
        [Inject] private IMovementsService _movementsService;
        [Inject] private IBattleTurnsService _battleTurnsService;

        private readonly CompositeDisposable _disposables = new();

        private enum WeaponType { Melee, Ranged, BrokenRanged }
        private readonly Dictionary<WeaponType, GameObject> _pool = new();

        private DomainGameObject _currentActiveGameObject;
        private TileView _currentHighlightedTile;
        private HashSet<Tile> _reachableTiles;

        private void OnEnable()
        {
            BindStreams();
            PreloadPreviewInstances();
        }

        private void OnDestroy() => _disposables.Dispose();

        public TileView GetHighlightedTile() => _currentHighlightedTile;

        private void PreloadPreviewInstances()
        {
            _pool[WeaponType.Melee] = Instantiate(_swordPrefab, transform);
            _pool[WeaponType.Ranged] = Instantiate(_arrowPrefab, transform);
            _pool[WeaponType.BrokenRanged] = Instantiate(_brokenArrowPrefab, transform);

            foreach (var item in _pool)
            {
                item.Value.SetActive(false);
            }
        }

        private void BindStreams()
        {
            _battleTurnsService.OnTurnStarted
                .Subscribe(currentActiveGameObject => _currentActiveGameObject = currentActiveGameObject)
                .AddTo(_disposables);

            _movementsService.OnReachableTilesReceived
                .Subscribe(reachableTiles =>
                {
                    _reachableTiles = reachableTiles.ToHashSet();
                    AddCurrentActiveGameObjectTile();
                })
                .AddTo(_disposables);
        }

        public void Hide()
        {
            if (_currentHighlightedTile != null)
            {
                _currentHighlightedTile.SelectedTileHighlight(false);
            }
            _currentHighlightedTile = null;

            foreach (var item in _pool.Values)
            {
                item.SetActive(false);
            }
        }

        public void PreviewMeleeAttack(Guid unitId, Vector2 pointerWorldPosition)
        {
            PreviewAttack(unitId, pointerWorldPosition, WeaponType.Melee);
        }

        public void PreviewRangedAttack(Guid unitId, Vector2 pointerWorldPosition, bool directShot = false)
        {
            var weaponType = directShot
                ? WeaponType.BrokenRanged
                : WeaponType.Ranged;

            PreviewAttack(unitId, pointerWorldPosition, weaponType);
        }

        private void PreviewAttack(Guid unitId, Vector2 pointerWorldPosition, WeaponType weaponType)
        {
            if (!_pool.TryGetValue(weaponType, out var weaponGameObject))
            {
                return;
            }

            foreach (var item in _pool)
            {
                if (item.Key != weaponType)
                {
                    item.Value.SetActive(false);
                }
            }

            if (!TryGetAttackOrigin(unitId, pointerWorldPosition, out var tileView, out var orientation))
            {
                Hide();
                return;
            }

            if (_currentHighlightedTile != tileView)
            {
                if (_currentHighlightedTile != null)
                {
                    _currentHighlightedTile.SelectedTileHighlight(false);
                }
                tileView.SelectedTileHighlight(true);
                _currentHighlightedTile = tileView;
            }

            weaponGameObject.transform.SetPositionAndRotation(
                tileView.transform.position,
                Quaternion.Euler(0, 0, orientation.RotationZ)
            );
            weaponGameObject.SetActive(true);
        }

        private void AddCurrentActiveGameObjectTile()
        {
            if (_currentActiveGameObject != null && _currentActiveGameObject is Unit unit)
            {
                var tile = _grid.GetTile(unit.Id);
                _reachableTiles.Add(tile.Data);
            }
        }

        private bool TryGetAttackOrigin(
            Guid unitId,
            Vector2 pointerWorldPosition,
            out TileView tileView,
            out AttackOrientation orientation)
        {
            var tile = _grid.GetTile(unitId);
            var tx = tile.Data.X;
            var ty = tile.Data.Y;
            var centerPosition = (Vector2)tile.transform.position;

            var candidates = new List<(int dx, int dy, TileView view)>();
            foreach (var (dx, dy) in new[]
            {
                (1, 0),(1,-1),(0,-1),(-1,-1),
                (-1, 0),(-1, 1),(0, 1),(1, 1)
            })
            {
                var neighbourTile = _grid.GetTile(tx + dx, ty + dy);
                if (neighbourTile != null && _reachableTiles.Contains(neighbourTile.Data))
                {
                    candidates.Add((dx, dy, neighbourTile));
                }
            }
            if (candidates.Count == 0)
            {
                tileView = null;
                orientation = default;
                return false;
            }

            var direction = (pointerWorldPosition - centerPosition).normalized;
            var bestCandidate = candidates
                .OrderByDescending(candidate => Vector2.Dot(direction, new Vector2(candidate.dx, candidate.dy).normalized))
                .First();

            tileView = bestCandidate.view;
            orientation = AttackOrientationHelper.FromPositions(tileView.transform.position, centerPosition);

            return true;
        }
    }
}
