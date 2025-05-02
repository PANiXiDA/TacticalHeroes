using UnityEngine.EventSystems;
using UnityEngine;
using R3;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class ButtonInput : MonoBehaviour, IPointerClickHandler
    {
        private readonly Subject<PointerEventData.InputButton> _click = new();

        public Observable<PointerEventData.InputButton> OnClick => _click.AsObservable();

        void IPointerClickHandler.OnPointerClick(PointerEventData e) => _click.OnNext(e.button);

        private void OnDestroy()
        {
            _click.Dispose();
        }
    }
}
