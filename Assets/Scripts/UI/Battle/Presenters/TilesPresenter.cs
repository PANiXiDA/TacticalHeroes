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
            CacheComponents();
            BindStreams();
            SetupDesktopHover();
            SetupMobileHover();
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
                .Subscribe(id => _view.ActiveGameObjectHighlight(id == _view.Data.OccupiedUnitId))
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
        }

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

        private void SetupClick()
        {
            _input.OnClick
                .Where(_ => _isReachable)
                .Subscribe(_ => _movementsService.GetPathAsync(_view.Data).Forget())
                .AddTo(_disposables);
        }

    }
}
