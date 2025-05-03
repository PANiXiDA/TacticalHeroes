using Assets.Scripts.UI.Battle.Core;
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

        private readonly CompositeDisposable _disposables = new();

        private UnitInfoView _view;

        private void OnEnable()
        {
            CacheComponents();
            SetupClick();
        }

        public void Show(Unit unit) => _view.Show(unit);
        public void Hide() => _view.Hide();

        private void CacheComponents()
        {
            _view = GetComponent<UnitInfoView>();
        }

        private void SetupClick()
        {
            _globalInput.OnClick
                .Subscribe(_ => Hide())
                .AddTo(_disposables);
        }
    }
}
