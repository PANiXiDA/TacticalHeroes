using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class BattleEndView : MonoBehaviour
    {
        [SerializeField] private Image _battleEndPanel;
        [SerializeField] private TextMeshProUGUI _winSideText;
        [SerializeField] private TextMeshProUGUI _loseSideText;

        public void SetActive(bool isActive) => _battleEndPanel.gameObject.SetActive(isActive);

        public void SetTexts(string winners, string losers)
        {
            _winSideText.text = winners;
            _loseSideText.text = losers;
        }
    }
}
