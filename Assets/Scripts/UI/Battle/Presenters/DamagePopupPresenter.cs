using Assets.Scripts.Domain.DTO.Models;

using Assets.Scripts.UI.Battle.Views;

using UnityEngine;

using Zenject;

namespace Assets.Scripts.UI.Battle.Presenters
{
    public sealed class DamagePopupPresenter : MonoBehaviour
    {
        private const string DamagePopupsContainerName = "DamagePopupsContainer";

        [SerializeField] private Canvas _canvas;
        [SerializeField] private DamagePopupView _damagePopupPrefab;

        [Inject] private readonly DiContainer _container;

        private RectTransform _damagePopupsContainer;

        private void OnEnable()
        {
            CreateDamagePopupContainer();
        }

        public void CreateDamagePopup(AttackEvent attackEvent, Vector2 screenPos)
        {
            var view = _container.InstantiatePrefabForComponent<DamagePopupView>(_damagePopupPrefab, _damagePopupsContainer);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_canvas.transform,
                screenPos,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
                out Vector2 localPosition);

            view.Init(localPosition, attackEvent.Damage, attackEvent.Deaths);
        }

        private void CreateDamagePopupContainer()
        {
            var gameObject = new GameObject(DamagePopupsContainerName, typeof(RectTransform));
            _damagePopupsContainer = gameObject.GetComponent<RectTransform>();
            _damagePopupsContainer.SetParent(_canvas.transform, false);
            _damagePopupsContainer.anchorMin = Vector2.zero;
            _damagePopupsContainer.anchorMax = Vector2.one;
            _damagePopupsContainer.offsetMin = Vector2.zero;
            _damagePopupsContainer.offsetMax = Vector2.zero;
        }
    }
}
