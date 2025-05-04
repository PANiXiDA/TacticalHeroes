using Assets.Scripts.UI.Battle.Core;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;

using R3;

using UnityEngine;

using Zenject;

using Unit = Assets.Scripts.GameEngine.Domain.Unit;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(UnitInfoView))]
    public sealed class UnitInfoPresenter : MonoBehaviour
    {
        [Inject] private readonly GlobalInput _globalInput;

        [SerializeField] private UnitInfoInput _infoInput;
        [SerializeField] private UnitInfoInput _effectsInput;

        private readonly CompositeDisposable _disposables = new();

        private Canvas _canvas;
        private UnitInfoView _view;
        private Unit _currentUnit;

        private void OnEnable()
        {
            CacheComponents();
            SetupClick();
        }

        public void Show(Unit unit)
        {
            _currentUnit = unit;
            _view.ShowInfo(unit);
        }

        private void CacheComponents()
        {
            _view = GetComponent<UnitInfoView>();
            _canvas = _infoInput.GetComponentInParent<Canvas>(true);
        }

        private void SetupClick()
        {
            _infoInput.OnClick
                .Subscribe(_ => _view.ShowEffects(_currentUnit))
                .AddTo(_disposables);

            _effectsInput.OnClick
                .Subscribe(_ => _view.ShowInfo(_currentUnit))
                .AddTo(_disposables);

            _globalInput.OnClick
                .Subscribe(_ => TryHidePanels())
                .AddTo(_disposables);
        }

        private void TryHidePanels()
        {
            Vector2 screenPos = Input.mousePosition;
            Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;

            bool insideInfo = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)_infoInput.transform, screenPos, cam);
            bool insideEff = RectTransformUtility.RectangleContainsScreenPoint((RectTransform)_effectsInput.transform, screenPos, cam);

            if (!insideInfo && !insideEff)
            {
                _view.Hide();
            }
        }
    }
}
