using R3;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class KeyboardInput : MonoBehaviour
    {
        private readonly Subject<bool> _shiftHeld = new();
        public Observable<bool> OnShiftHeld => _shiftHeld.AsObservable();

        private InputAction _shiftAction;

        private void OnEnable()
        {
            _shiftAction = new InputAction(
                name: "ShiftHold",
                type: InputActionType.Button,
                binding: "<Keyboard>/shift");

            _shiftAction.performed += _ => _shiftHeld.OnNext(true);
            _shiftAction.canceled += _ => _shiftHeld.OnNext(false);
            _shiftAction.Enable();
        }

        private void OnDisable()
        {
            _shiftAction?.Dispose();
            _shiftHeld.OnCompleted();
        }
    }
}
