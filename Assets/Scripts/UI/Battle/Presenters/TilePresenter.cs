using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;
using UnityEngine;
using R3;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(TileInput))]
    [RequireComponent(typeof(TileView))]
    public sealed class TilePresenter : MonoBehaviour
    {
        private TileInput _input;
        private TileView _view;

        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            _input = GetComponent<TileInput>();
            _view = GetComponent<TileView>();

            SetupDesktopHover();
            SetupMobileHover();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void SetupDesktopHover()
        {
            _input.OnEnter
                .Subscribe(_ => _view.SelectedTileHighlight(true))
                .AddTo(_disposables);

            _input.OnExit
                .Subscribe(_ => _view.SelectedTileHighlight(false))
                .AddTo(_disposables);
        }

        private void SetupMobileHover()
        {
            _input.OnDown
                .Subscribe(_ => _view.SelectedTileHighlight(true))
                .AddTo(_disposables);

            _input.OnUp
                .Subscribe(_ => _view.SelectedTileHighlight(false))
                .AddTo(_disposables);
        }
    }
}
