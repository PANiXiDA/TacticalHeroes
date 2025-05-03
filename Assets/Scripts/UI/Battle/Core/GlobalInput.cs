using R3;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Battle.Core
{
    public sealed class GlobalInput : MonoBehaviour
    {
        private readonly Subject<PointerEventData.InputButton> _click = new();
        public Observable<PointerEventData.InputButton> OnClick => _click.AsObservable();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _click.OnNext(PointerEventData.InputButton.Left);
            }

            if (Input.GetMouseButtonDown(1))
            {
                _click.OnNext(PointerEventData.InputButton.Right);
            }
        }
    }
}
