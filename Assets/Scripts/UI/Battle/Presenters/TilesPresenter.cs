using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;
using UnityEngine;
using R3;
using Assets.Scripts.Services.Interfaces.Battle;
using Zenject;
using System.Linq;
using Cysharp.Threading.Tasks;
using Assets.Scripts.Common.Enumerations;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(TileInput))]
    [RequireComponent(typeof(TileView))]
    public sealed class TilesPresenter : MonoBehaviour
    {
        private TileInput _input;
        private TileView _view;

        private bool _isReachable;

        [Inject] private readonly TileInfoPresenter _tileInfoPresenter;

        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IMovementsService _movementsService;
        [Inject] private readonly IButtonStatesService _buttonStatesService;
        [Inject] private readonly IBattleActionsFacade _battleActionsFacade;

        private readonly CompositeDisposable _disposables = new();

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
            _input = GetComponent<TileInput>();
            _view = GetComponent<TileView>();
        }

        private void BindStreams()
        {
            _battleTurnsService.OnTurnStarted
                .Subscribe(currentActiveGameObject => _view.ActiveGameObjectHighlight(currentActiveGameObject.Id == _view.Data.OccupiedUnitId))
                .AddTo(_disposables);

            _movementsService.OnReachableTilesReceived
                .Select(list => list.Contains(_view.Data))
                .DistinctUntilChanged()
                .Subscribe(flag =>
                {
                    _isReachable = flag;
                    _view.TileForMoveHighlight(flag);
                })
                .AddTo(_disposables);

            _movementsService.OnPathComputed
                .Subscribe(_ => _view.ActiveGameObjectHighlight(false))
                .AddTo(_disposables);
        }

        private void SetupHover()
        {
            _input.OnEnter
                .Where(_ => _isReachable && !IsInfoClicked())
                .Subscribe(_ => _view.SelectedTileHighlight(_isReachable))
                .AddTo(_disposables);

            _input.OnEnter
                .Subscribe(_ => _tileInfoPresenter.ShowCoordinates(_view.Data.X, _view.Data.Y))
                .AddTo(_disposables);

            _input.OnExit
                .Subscribe(_ => 
                { 
                    _view.SelectedTileHighlight(false);
                    _tileInfoPresenter.ClearInfo();
                })
                .AddTo(_disposables);
        }

        private void SetupClick()
        {
            _input.OnClick
                .Where(button => button == PointerEventData.InputButton.Left && _isReachable && !IsInfoClicked())
                .Subscribe(_ => _battleActionsFacade.MoveAsync(_view.Data).Forget())
                .AddTo(_disposables);
        }

        private bool IsInfoClicked() => _buttonStatesService.IsActive(BattleButtonType.Info);
    }
}
