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
        [Inject] private readonly IBattleTurnsService _battleTurnsService;
        [Inject] private readonly IATBService _atbService;
        [Inject] private readonly KeyboardInput _keyboardInput;
        [Inject] private readonly SurrenderPresenter _surrenderPresenter;

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
            if (_view.GetButtonType() == BattleButtonType.MeleeAttack)
            {
                _keyboardInput.OnShiftHeld
                    .Subscribe(held => _buttonStatesService.Set(BattleButtonType.MeleeAttack, held))
                    .AddTo(_disposables);
            }

            _buttonStatesService.OnStateChanged
                .Where(item => item.Type == _view.GetButtonType())
                .Subscribe(item => _view.SetActive(item.IsActive))
                .AddTo(_disposables);

            _input.OnClick
                .Subscribe(_ => HandleClick(_view.GetButtonType()))
                .AddTo(_disposables);

            _input.OnEnter
              .Subscribe(_ => HandleEnter(_view.GetButtonType()))
              .AddTo(_disposables);

            _input.OnExit
              .Subscribe(_ => HandleExit(_view.GetButtonType()))
              .AddTo(_disposables);
        }

        private void HandleClick(BattleButtonType buttonType)
        {
            _buttonStatesService.Toggle(buttonType);
            switch (buttonType)
            {
                case BattleButtonType.Exit:
                    _surrenderPresenter.Show();
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

        private void HandleEnter(BattleButtonType type)
        {
            switch (type)
            {
                case BattleButtonType.Exit:
                    break;
                case BattleButtonType.Info:
                    break;
                case BattleButtonType.Wait:
                    var active = _battleTurnsService.GetCurrentActiveGameObject();
                    _atbService.BuildWaitPreview(activeGameObjectId: active.Id, isWait: true);
                    break;
                case BattleButtonType.Defence:
                    break;
                case BattleButtonType.UseAbility:
                    break;
                case BattleButtonType.OpenMagicBook:
                    break;
                case BattleButtonType.MeleeAttack:
                    break;
            }
        }

        private void HandleExit(BattleButtonType type)
        {
            switch (type)
            {
                case BattleButtonType.Exit:
                    break;
                case BattleButtonType.Info:
                    break;
                case BattleButtonType.Wait:
                    _atbService.CancelWaitPreview();
                    break;
                case BattleButtonType.Defence:
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
