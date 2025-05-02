using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class TileInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _infoText;

        public void ShowInfo(string text) => _infoText.text = text; 
    }
}
