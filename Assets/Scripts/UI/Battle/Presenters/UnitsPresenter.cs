using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Domain.DTO.Models;
using Assets.Scripts.GameEngine.Domain.Enums;
using Assets.Scripts.GameEngine.DTO.PathFinderCalculator;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;

using Cysharp.Threading.Tasks;

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
        [Inject] private readonly GridsPresenter _grid;
        [Inject] private readonly AttackPreviewsPresenter _attackPreviews;
        [Inject] private readonly UnitInfoPresenter _unitInfo;

        [Inject] private readonly IButtonStatesService _buttonStatesService;
        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IMovementsService _movementsService;
        [Inject] private readonly IBattleActionsFacade _battleActionsFacade;

        private readonly CompositeDisposable _disposables = new();

        private UnitInput _input;
        private UnitView _view;

        private void OnEnable()
        {
            CacheComponents();
            BindStreams();
            SetupHover();
            SetupClick();
            SetupInfo();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void CacheComponents()
        {
            _input = GetComponent<UnitInput>();
            _view = GetComponent<UnitView>();
        }

        private void BindStreams()
        {
            _movementsService.OnPathComputed
                .Where(path => path.UnitId == _view.Data.Id)
                .Subscribe(path => HandleMove(path.Tiles))
                .AddTo(_disposables);

            _battleActionsFacade.OnAttackDone
                .Subscribe(attackEvent => AttackDone(attackEvent))
                .AddTo(_disposables);
        }

        private void SetupHover()
        {
            _input.OnMove
                .Where(_ => IsEnemyTarget() && !IsInfoClicked())
                .Subscribe(evt => HandleHover(evt))
                .AddTo(_disposables);

            _input.OnExit
                .Subscribe(_ => _attackPreviews.Hide())
                .AddTo(_disposables);
        }

        private void SetupClick()
        {
            _input.OnClick
                .Where(button => button == PointerEventData.InputButton.Left && IsEnemyTarget() && !IsInfoClicked())
                .Subscribe(_ => HandleClick().Forget())
                .AddTo(_disposables);
        }

        private void SetupInfo()
        {
            _input.OnClick
                .Where(button => button == PointerEventData.InputButton.Right || IsInfoClicked())
                .Subscribe(_ => _unitInfo.Show(_view.Data))
                .AddTo(_disposables);
        }

        private bool IsEnemyTarget() => _battleTurnsService.GetCurrentActiveGameObject().OwnerId != _view.Data.OwnerId;
        private bool IsInfoClicked() => _buttonStatesService.IsActive(BattleButtonType.Info);

        private void HandleHover(PointerEventData evt)
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

        private async UniTaskVoid HandleClick()
        {
            var tileView = _attackPreviews.GetHighlightedTile();
            if (tileView == null)
            {
                return;
            }

            await _battleActionsFacade.MeleeAttackAsync(_view.Data, tileView.Data);
        }

        private void HandleMove(IReadOnlyList<Tile> tiles)
        {
            var path = tiles
                .Select(t => _grid.GetTile(t.X, t.Y).transform.position)
                .ToArray();

            _view.MoveAsync(path, () =>
            {
                _movementsService.NotifyMovementCompleted(_view.Data.Id);
            }).Forget();
        }

        private void AttackDone(AttackEvent attackEvent)
        {
            if (attackEvent.Attacker.Id == _view.Data.Id)
            {
                HandleAttack(attackEvent.Defender);
            }
            if (attackEvent.Defender.Id == _view.Data.Id && attackEvent.Attacker is Unit attacker)
            {
                HandleDefend(attacker);
            }
        }

        private void HandleAttack(Unit defender)
        {
            var defenderTile = _grid.GetTile(defender.Id);
            var defenderPosition = defenderTile.transform.position;
            _view.AttackAsync(defenderPosition).Forget();
        }

        private void HandleDefend(Unit attacker)
        {
            var attackerTile = _grid.GetTile(attacker.Id);
            var attackerPosition = attackerTile.transform.position;
            
            if (_view.Data.Count > 0)
            {
                _view.TakeDamageAsync(attackerPosition).Forget();
            }
            else
            {
                _view.DeathAsync(attackerPosition).Forget();
            }
        }
    }
}
