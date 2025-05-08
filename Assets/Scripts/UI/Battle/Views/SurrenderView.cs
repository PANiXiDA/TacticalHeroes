using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class SurrenderView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _surrenderPanel;

        public void SetActive(bool isActive) => _surrenderPanel.gameObject.SetActive(isActive);
    }
}
