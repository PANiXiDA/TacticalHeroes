using R3;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class UnitInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        private readonly Subject<PointerEventData.InputButton> _click = new();
        private readonly Subject<Unit> _enter = new();
        private readonly Subject<Unit> _exit = new();
        private readonly Subject<Unit> _down = new();
        private readonly Subject<Unit> _up = new();
        private readonly Subject<PointerEventData> _move = new();

        public Observable<PointerEventData.InputButton> OnClick => _click.AsObservable();
        public Observable<Unit> OnEnter => _enter.AsObservable();
        public Observable<Unit> OnExit => _exit.AsObservable();
        public Observable<Unit> OnDown => _down.AsObservable();
        public Observable<Unit> OnUp => _up.AsObservable();
        public Observable<PointerEventData> OnMove => _move.AsObservable();

        void IPointerClickHandler.OnPointerClick(PointerEventData e) => _click.OnNext(e.button);
        void IPointerEnterHandler.OnPointerEnter(PointerEventData _) => _enter.OnNext(Unit.Default);
        void IPointerExitHandler.OnPointerExit(PointerEventData _) => _exit.OnNext(Unit.Default);
        void IPointerDownHandler.OnPointerDown(PointerEventData e) => _down.OnNext(Unit.Default);
        void IPointerUpHandler.OnPointerUp(PointerEventData e) => _up.OnNext(Unit.Default);
        void IPointerMoveHandler.OnPointerMove(PointerEventData e) => _move.OnNext(e);

        private void OnDestroy()
        {
            _click.Dispose();
            _enter.Dispose();
            _exit.Dispose();
            _down.Dispose();
            _up.Dispose();
            _move.Dispose();
        }
    }
}
