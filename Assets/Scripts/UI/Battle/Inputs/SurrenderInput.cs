using R3;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class SurrenderInput : MonoBehaviour
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private readonly Subject<Unit> _yesClick = new();
        private readonly Subject<Unit> _noClick = new();

        public Observable<Unit> OnYes => _yesClick.AsObservable();
        public Observable<Unit> OnNo => _noClick.AsObservable();

        private void Awake()
        {
            _yesButton.onClick.AddListener(() => _yesClick.OnNext(Unit.Default));
            _noButton.onClick.AddListener(() => _noClick.OnNext(Unit.Default));
        }

        private void OnDestroy()
        {
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
            _yesClick.Dispose();
            _noClick.Dispose();
        }
    }
}
