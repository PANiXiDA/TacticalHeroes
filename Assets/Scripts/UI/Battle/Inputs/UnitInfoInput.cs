using R3;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class UnitInfoInput : MonoBehaviour, IPointerClickHandler
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
