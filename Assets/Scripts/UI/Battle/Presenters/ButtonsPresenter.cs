using Assets.Scripts.Common.Enumerations;
using Assets.Scripts.UI.Battle.Inputs;
using Assets.Scripts.UI.Battle.Views;

using R3;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(ButtonInput))]
    [RequireComponent(typeof(ButtonView))]
    public sealed class ButtonsPresenter : MonoBehaviour
    {
        private readonly CompositeDisposable _disposables = new();

        private ButtonInput _input;
        private ButtonView _view;

        private void OnEnable()
        {
            CacheComponents();
            SetupClick();
        }

        private void OnDestroy() => _disposables.Dispose();

        private void CacheComponents()
        {
            _input = GetComponent<ButtonInput>();
            _view = GetComponent<ButtonView>();
        }

        private void SetupClick()
        {
            _input.OnClick
                .Subscribe(_ => HandleClick(_view.GetButtonType))
                .AddTo(_disposables);
        }

        private void HandleClick(BattleButtonType type)
        {
            switch (type)
            {
                case BattleButtonType.Exit: break;
                case BattleButtonType.Info: break;
                case BattleButtonType.Wait: break;
                case BattleButtonType.Defense: break;
                case BattleButtonType.UseAbility: break;
                case BattleButtonType.OpenMagicBook: break;
                case BattleButtonType.MeleeAttack: break;
            }
        }
    }
}
