using R3;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Battle.Inputs
{
    public sealed class BattleEndInput : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;

        private readonly Subject<Unit> _continueClick = new();

        public Observable<Unit> OnContinue => _continueClick.AsObservable();

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => _continueClick.OnNext(Unit.Default));
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveAllListeners();
            _continueClick.Dispose();
        }
    }
}
