using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.UI.Battle.Extensions
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridCellSizeAdaptiveScaler : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;

        private GridLayoutGroup _grid;

        private void Awake()
        {
            _grid = GetComponent<GridLayoutGroup>();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_container != null && _grid != null)
            {
                _grid.cellSize = new Vector2(_container.rect.width, _container.rect.width);
            }
        }
    }

}
