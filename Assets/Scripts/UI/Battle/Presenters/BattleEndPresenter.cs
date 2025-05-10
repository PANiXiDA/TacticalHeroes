using Assets.Scripts.Common.Constants;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;

using R3;

using UnityEngine;
using UnityEngine.SceneManagement;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(BattleEndInput))]
    [RequireComponent(typeof(BattleEndView))]
    public sealed class BattleEndPresenter : MonoBehaviour
    {
        [Inject] private readonly IBattleEndService _battleEndService;

        private readonly CompositeDisposable _disposables = new();

        private BattleEndInput _input;
        private BattleEndView _view;

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
            _input = GetComponent<BattleEndInput>();
            _view = GetComponent<BattleEndView>();
        }

        private void BindStreams()
        {
            _battleEndService.OnBattleEndMessageBuilt
                .Subscribe(message =>
                {
                    _view.SetTexts(message.Winners, message.Losers);
                    _view.SetActive(true);
                })
                .AddTo(_disposables);

            _input.OnContinue
              .Subscribe(_ => SceneManager.LoadScene(SceneConstants.MenuScene))
              .AddTo(_disposables);
        }
    }
}
