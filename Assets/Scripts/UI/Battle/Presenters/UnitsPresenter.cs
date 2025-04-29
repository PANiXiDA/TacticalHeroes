using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Views;

using R3;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(UnitView))]
    public sealed class UnitsPresenter : MonoBehaviour
    {
        private UnitView _view;

        [Inject] private readonly IBattleTurnsService _battleTurnsService;

        private readonly CompositeDisposable _disposables = new();

        private void OnEnable()
        {
            _view = GetComponent<UnitView>();

            //_battleTurnsService.OnTurnStarted
            //    .Subscribe(id => _view.Data.Id == id)
            //    .AddTo(_disposables);
        }

        private void OnDestroy() => _disposables.Dispose();


    }
}
