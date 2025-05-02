using Assets.Scripts.UI.Battle.Views;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Presenters
{
    [RequireComponent(typeof(TileInfoView))]
    public sealed class TileInfoPresenter : MonoBehaviour
    {
        private TileInfoView _view;

        private void OnEnable()
        {
            CacheComponents();
        }

        private void CacheComponents()
        {
            _view = GetComponent<TileInfoView>();
        }

        public void ShowCoordinates(int x, int y)
        {
            var info = $"X:{x}\nY:{y}";
            _view.ShowInfo(info);
        }

        public void ClearInfo()
        {
            _view.ShowInfo(string.Empty);
        }
    }
}
