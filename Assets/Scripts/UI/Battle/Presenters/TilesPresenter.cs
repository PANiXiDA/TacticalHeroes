using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;
using UnityEngine;
using R3;
using Assets.Scripts.Services.Interfaces.Battle;
using Zenject;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(TileInput))]
    [RequireComponent(typeof(TileView))]
    public sealed class TilesPresenter : MonoBehaviour
    {
        private TileInput _input;
        private TileView _view;

        private bool _isReachable;

        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IMovementsService _movementsService;

        private readonly CompositeDisposable _disposables = new();

        private void OnEnable()
        {
            _input = GetComponent<TileInput>();
            _view = GetComponent<TileView>();

            _battleTurnsService.OnTurnStarted
                .Subscribe(currentActiveGameObjectId => _view.ActiveGameObjectHighlight(currentActiveGameObjectId == _view.Data.OccupiedUnitId))
                .AddTo(_disposables);

            _movementsService.OnReachableTilesReceived
                .Select(tiles => tiles.Contains(_view.Data))
                .DistinctUntilChanged()
                .Subscribe(flag =>
                {
                    _isReachable = flag;
                    _view.TileForMoveHighlight(flag);
                })
                .AddTo(_disposables);

            SetupDesktopHover();
            SetupMobileHover();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void SetupDesktopHover()
        {
            _input.OnEnter
                .Where(_ => _isReachable)
                .Subscribe(_ => _view.SelectedTileHighlight(_isReachable))
                .AddTo(_disposables);

            _input.OnExit
                .Subscribe(_ => _view.SelectedTileHighlight(false))
                .AddTo(_disposables);
        }

        private void SetupMobileHover()
        {
            _input.OnDown
                .Where(_ => _isReachable)
                .Subscribe(_ => _view.SelectedTileHighlight(_isReachable))
                .AddTo(_disposables);

            _input.OnUp
                .Subscribe(_ => _view.SelectedTileHighlight(false))
                .AddTo(_disposables);
        }
    }
}
