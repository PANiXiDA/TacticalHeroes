using R3;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Assets.Scripts.UI.Battle.Core
{
    public readonly struct ClickInfo
    {
        public PointerEventData.InputButton Button { get; }
        public Vector2 ScreenPos { get; }

        public ClickInfo(PointerEventData.InputButton btn, Vector2 pos)
        {
            Button = btn;
            ScreenPos = pos;
        }
    }

    public sealed class GlobalInput : MonoBehaviour
    {
        private readonly Subject<ClickInfo> _click = new();
        public Observable<ClickInfo> OnClick => _click.AsObservable();

        private InputAction _left;
        private InputAction _right;

        private void OnEnable()
        {
            _left = new InputAction(
                name: "PrimaryClick",
                type: InputActionType.Button,
                binding: "<Pointer>/press");
            _left.performed += ctx =>
            {
                var pos = Pointer.current.position.ReadValue();
                _click.OnNext(new ClickInfo(PointerEventData.InputButton.Left, pos));
            };
            _left.Enable();

            _right = new InputAction(
                name: "RightClick",
                type: InputActionType.Button,
                binding: "<Mouse>/rightButton");
            _right.performed += _ =>
            {
                var pos = Pointer.current.position.ReadValue();
                _click.OnNext(new ClickInfo(PointerEventData.InputButton.Right, pos));
            };
            _right.Enable();
        }

        private void OnDisable()
        {
            _left.Dispose();
            _right.Dispose();
            _click.OnCompleted();
        }
    }
}
