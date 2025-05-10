using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class SurrenderView : MonoBehaviour
    {
        [SerializeField] private Image _surrenderPanel;

        public void SetActive(bool isActive) => _surrenderPanel.gameObject.SetActive(isActive);
    }
}
