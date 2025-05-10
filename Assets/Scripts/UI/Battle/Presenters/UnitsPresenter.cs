using System;
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
        [Inject] private readonly IAttacksService _attacksService;
        [Inject] private readonly IBattleActionsFacade _battleActionsFacade;
        [Inject] private readonly IATBService _atbService;
        [Inject] private readonly ISurrenderService _surrenderService;

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

            _attacksService.OnAttackPrepared
                .Subscribe(attackEvent => AttackDone(attackEvent).Forget())
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

            _surrenderService.OnSurrenderUnitsGot
                .Where(ids => ids.Contains(_view.Data.Id))
                .Subscribe(_ => HandleSurrenderDeath().Forget())
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

        private bool CanMakeRangedAttack(Unit unit)
        {
            var isMeleeForced = _buttonStatesService.IsActive(BattleButtonType.MeleeAttack);
            if (isMeleeForced)
            {
                return false;
            }

            var isRangedUnit = unit.Abilities.Any(ability => ability.Type == AbilityType.Archer);
            if (!isRangedUnit)
            {
                return false;
            }

            if (!unit.Arrows.HasValue || unit.Arrows <= 0)
            {
                return false;
            }

            var tile = _grid.GetTile(unit.Id);
            var hasMeleeEnemyNeighbour = _grid
                .GetNeighbours(tile.Data.X, tile.Data.Y)
                .Select(tile => tile.Data.OccupiedUnitId)
                .Where(id => id != Guid.Empty && id.HasValue)
                .Select(id =>
                {
                    _atbService.TryGetGameObject(id.Value, out var obj);
                    return obj as Unit;
                })
                .Where(adjacentUnit => adjacentUnit != null)
                .Any(adjacentUnit => adjacentUnit.OwnerId != unit.OwnerId);
            if (hasMeleeEnemyNeighbour)
            {
                return false;
            }


            return true;
        }

        private bool IsDirectShoot(Unit attacker, Unit defender)
        {
            if (!attacker.Range.HasValue)
            {
                return false;
            }

            var attackerTile = _grid.GetTile(attacker.Id);
            var defenderTile = _grid.GetTile(defender.Id);

            var distance = _grid.GetDistance(attackerTile.Data, defenderTile.Data);

            return attacker.Range.Value >= distance;
        }

        private void HandleHover(PointerEventData evt)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(evt.position);

            var activeGameObject = _battleTurnsService.GetCurrentActiveGameObject();
            var isRangeAttack = activeGameObject switch
            {
                Unit unit => CanMakeRangedAttack(unit),
                _ => true
            };

            if (isRangeAttack)
            {
                var directShot = activeGameObject switch
                {
                    Unit unit => IsDirectShoot(unit, _view.Data),
                    _ => true
                };

                _attackPreviews.PreviewRangedAttack(_view.Data.Id, worldPos, directShot);
            }
            else
            {
                _attackPreviews.PreviewMeleeAttack(_view.Data.Id, worldPos);
            }
        }

        private async UniTaskVoid HandleClick()
        {
            var attacker = _battleTurnsService.GetCurrentActiveGameObject();

            bool canRange = attacker is Unit rangedUnit && CanMakeRangedAttack(rangedUnit);

            if (canRange)
            {
                await _battleActionsFacade.RangeAttackAsync(_view.Data);
            }
            else
            {
                var tileView = _attackPreviews.GetHighlightedTile();
                if (tileView == null)
                {
                    return;
                }

                await _battleActionsFacade.MeleeAttackAsync(_view.Data, tileView.Data);
            }
        }

        private async UniTask AttackDone(List<AttackEvent> attackEvents)
        {
            var rootId = attackEvents.First().Attacker.Id;

            foreach (var attackEvent in attackEvents)
            {
                if (attackEvent.Attacker.Id == _view.Data.Id)
                {
                    await HandleAttack(attackEvent.Defender, attackEvent.IsRangeAttack);
                }
                if (attackEvent.Defender.Id == _view.Data.Id && attackEvent.Attacker is Unit attacker)
                {
                    await HandleDefend(attacker);
                }
            }

            if (_view.Data.Id == rootId)
            {
                _attacksService.NotifyAttackCompleted(rootId);
            }
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

        private async UniTask HandleAttack(Unit defender, bool isRangeAttack)
        {
            var defenderTile = _grid.GetTile(defender.Id);
            var defenderPosition = defenderTile.transform.position;
            await _view.AttackAsync(defenderPosition, isRangeAttack);
        }

        private async UniTask HandleDefend(Unit attacker)
        {
            var attackerTile = _grid.GetTile(attacker.Id);
            var attackerPosition = attackerTile.transform.position;
            
            if (_view.Data.Count > 0)
            {
                await _view.TakeDamageAsync(attackerPosition);
            }
            else
            {
                await _view.DeathAsync(attackerPosition);
            }
        }

        private async UniTask HandleSurrenderDeath()
        {
            await _view.DeathAsync(transform.position);
        }
    }
}
