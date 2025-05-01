using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Enums;
using Assets.Scripts.GameEngine.DTO.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using R3;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

using DomainGameObject = Assets.Scripts.GameEngine.Domain.Core.GameObject;
using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(UnitInput))]
    [RequireComponent(typeof(UnitView))]
    public sealed class UnitsPresenter : MonoBehaviour
    {
        private const float MoveSpeed = 5f;

        [Inject] private GridsPresenter _grid;
        [Inject] private AttackPreviewsPresenter _attackPreviews;

        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IMovementsService _movementsService;
        [Inject] private readonly IAttacksService _attacksService;

        private readonly CompositeDisposable _disposables = new();

        private UnitInput _input;
        private UnitView _view;

        private DomainGameObject _currentActiveGameObject;

        private void OnEnable()
        {
            CacheComponents();
            BindStreams();
            SetupHover();
            SetupClick();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void CacheComponents()
        {
            _input = GetComponent<UnitInput>();
            _view = GetComponent<UnitView>();
        }

        private void BindStreams()
        {
            _battleTurnsService.OnTurnStarted
                .Subscribe(currentActiveGameObject => _currentActiveGameObject = currentActiveGameObject)
                .AddTo(_disposables);

            _movementsService.OnPathComputed
                .Where(path => path.UnitId == _view.Data.Id)
                .Subscribe(path => Move(path.Tiles))
                .AddTo(_disposables);

            _attacksService.OnAttackDone
                .Subscribe(attackEvent => AttackDone(attackEvent))
                .AddTo(_disposables);
        }

        private void SetupHover()
        {
            _input.OnMove
                .Where(_ => IsEnemyTarget())
                .Subscribe(evt => OnHover(evt))
                .AddTo(_disposables);

            _input.OnExit
                .Subscribe(_ => _attackPreviews.Hide())
                .AddTo(_disposables);
        }

        private void SetupClick()
        {
            _input.OnClick
                .Where(_ => IsEnemyTarget())
                .Subscribe(_ => OnClick().Forget())
                .AddTo(_disposables);
        }

        private bool IsEnemyTarget()
        {
            return _currentActiveGameObject.OwnerId != _view.Data.OwnerId;
        }

        private void OnHover(PointerEventData evt)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(evt.position);

            if (_view.Data.Abilities.Any(ability => ability.Type == AbilityType.Archer))
            {
                _attackPreviews.PreviewRangedAttack(_view.Data.Id, worldPos);
            }
            else
            {
                _attackPreviews.PreviewMeleeAttack(_view.Data.Id, worldPos);
            }
        }

        private async UniTaskVoid OnClick()
        {
            var tileView = _attackPreviews.GetHighlightedTile();
            await _attacksService.MeleeAttackAsync(_currentActiveGameObject, _view.Data, tileView.Data);
        }

        private void Move(IReadOnlyList<Tile> tiles)
        {
            var path = GetWorldPathPositions(tiles);
            var defaultFlip = PrepareViewForMove(path);
            var duration = CalculateDuration(path);
            AnimateMovement(path, duration, defaultFlip);
        }

        private Vector3[] GetWorldPathPositions(IReadOnlyList<Tile> tiles)
        {
            return tiles
                .Select(t => _grid.GetTile(t.X, t.Y).transform.position)
                .ToArray();
        }

        private bool PrepareViewForMove(Vector3[] path)
        {
            bool defaultFlip = _view.Side == PlayerSide.Right;
            float deltaX = path.Last().x - path.First().x;
            bool shouldFlip = (_view.Side == PlayerSide.Right && deltaX > 0) || (_view.Side == PlayerSide.Left && deltaX < 0);
            if (shouldFlip)
            {
                _view.Flip(!defaultFlip);
            }

            _view.SetMovingState(true);
            _view.IncrementSortingOrder();

            return defaultFlip;
        }

        private float CalculateDuration(Vector3[] path)
        {
            float totalDistance = 0f;
            for (int i = 1; i < path.Length; i++)
            {
                totalDistance += Vector3.Distance(path[i - 1], path[i]);
            }
            return totalDistance / MoveSpeed;
        }

        private void AnimateMovement(Vector3[] path, float duration, bool defaultFlip)
        {
            transform
                .DOPath(path, duration, PathType.Linear, PathMode.TopDown2D)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _view.SetMovingState(false);
                    _view.Flip(defaultFlip);
                    _view.DecrementSortingOrder();
                    _movementsService.NotifyMovementCompleted(_view.Data.Id);
                });
        }

        private void AttackDone(AttackEvent attackEvent)
        {
            if (attackEvent.Attacker.Id == _view.Data.Id && attackEvent.Attacker is Unit unit)
            {
                Attack(unit);
            }
            if (attackEvent.Defender.Id == _view.Data.Id)
            {
                HandleDefender(attackEvent.Defender);
            }
        }

        private void Attack(Unit unit)
        {
            _view.PlayFrontMeleeAttackAnimation();
        }

        private void HandleDefender(Unit defender)
        {
            if (_view.Data.Count > 0)
            {
                TakeDamage(defender);
            }
            else
            {
                Death(defender);
            }
        }

        private void TakeDamage(Unit unit)
        {
            _view.SetCount(unit.Count);
            _view.PlayTakeDamageAnimation();
        }

        private void Death(Unit unit)
        {
            _view.PlayDeathAnimation();
        }
    }
}
