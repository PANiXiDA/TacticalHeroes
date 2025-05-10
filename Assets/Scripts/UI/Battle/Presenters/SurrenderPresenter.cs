using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;
using R3;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(SurrenderInput))]
    [RequireComponent(typeof(SurrenderView))]
    public sealed class SurrenderPresenter : MonoBehaviour
    {
        [Inject] private readonly IBattleActionsFacade _battleActionsFacade;

        private readonly CompositeDisposable _disposables = new();

        private SurrenderInput _input;
        private SurrenderView _view;

        private void OnEnable()
        {
            CacheComponents();
            BindStreams();
        }

        private void OnDestroy() => _disposables.Dispose();

        public void Show() => _view.SetActive(true);
        public void Hide() => _view.SetActive(false);

        private void CacheComponents()
        {
            _input = GetComponent<SurrenderInput>();
            _view = GetComponent<SurrenderView>();
        }

        private void BindStreams()
        {
            _input.OnYes
              .Subscribe(_ => 
              {
                  _battleActionsFacade.SurrenderAsync();
                  Hide();
              })
              .AddTo(_disposables);

            _input.OnNo
                .Subscribe(_ => Hide())
                .AddTo(_disposables);
        }
    }
}
