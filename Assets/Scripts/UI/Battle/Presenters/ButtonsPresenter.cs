using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.Services.Interfaces.Battle;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;

using R3;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(ButtonInput))]
    [RequireComponent(typeof(ButtonView))]
    public sealed class ButtonsPresenter : MonoBehaviour
    {
        [Inject] private readonly IButtonStatesService _buttonStatesService;
        [Inject] private readonly IBattleActionsFacade _battleActionsFacade;

        private readonly CompositeDisposable _disposables = new();

        private ButtonInput _input;
        private ButtonView _view;

        private void OnEnable()
        {
            CacheComponents();
            BindStreams();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void CacheComponents()
        {
            _input = GetComponent<ButtonInput>();
            _view = GetComponent<ButtonView>();
        }

        private void BindStreams()
        {
            _input.OnClick
                .Subscribe(_ => HandleClick(_view.GetButtonType()))
                .AddTo(_disposables);

            _buttonStatesService.OnStateChanged
                .Where(item => item.Type == _view.GetButtonType())
                .Subscribe(item => _view.SetActive(item.IsActive))
                .AddTo(_disposables);
        }

        private void HandleClick(BattleButtonType buttonType)
        {
            _buttonStatesService.Toggle(buttonType);
            switch (buttonType)
            {
                case BattleButtonType.Exit:
                    break;
                case BattleButtonType.Info:
                    break;
                case BattleButtonType.Wait:
                    _battleActionsFacade.WaitAsync();
                    break;
                case BattleButtonType.Defence:
                    _battleActionsFacade.DefenceAsync();
                    break;
                case BattleButtonType.UseAbility:
                    break;
                case BattleButtonType.OpenMagicBook:
                    break;
                case BattleButtonType.MeleeAttack:
                    break;
            }
        }
    }
}
