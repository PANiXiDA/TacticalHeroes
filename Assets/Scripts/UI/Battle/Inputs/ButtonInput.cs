using UnityEngine.EventSystems;
using UnityEngine;
using R3;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class ButtonInput : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private readonly Subject<PointerEventData.InputButton> _click = new();
        private readonly Subject<Unit> _enter = new();
        private readonly Subject<Unit> _exit = new();

        public Observable<PointerEventData.InputButton> OnClick => _click.AsObservable();
        public Observable<Unit> OnEnter => _enter.AsObservable();
        public Observable<Unit> OnExit => _exit.AsObservable();

        void IPointerClickHandler.OnPointerClick(PointerEventData e) => _click.OnNext(e.button);
        void IPointerEnterHandler.OnPointerEnter(PointerEventData _) => _enter.OnNext(Unit.Default);
        void IPointerExitHandler.OnPointerExit(PointerEventData _) => _exit.OnNext(Unit.Default);

        private void OnDestroy()
        {
            _click.Dispose();
        }
    }
}
