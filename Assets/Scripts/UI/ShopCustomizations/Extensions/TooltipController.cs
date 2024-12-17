using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.UI.ShopCustomizations.Tooltip
{
    public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject tooltipPanel;
        public float longPressDuration = 0.5f;

        private bool isPressing = false;
        private float pressTime = 0f;

        void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (isPressing)
                {
                    pressTime += Time.deltaTime;
                    if (pressTime >= longPressDuration)
                    {
                        ShowTooltip();
                        isPressing = false;
                    }
                }

                CheckForOutsideTouch();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Application.isMobilePlatform)
                ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Application.isMobilePlatform)
                HideTooltip();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Application.isMobilePlatform)
            {
                isPressing = true;
                pressTime = 0f;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Application.isMobilePlatform)
            {
                isPressing = false;
                pressTime = 0f;
                HideTooltip();
            }
        }

        private void ShowTooltip()
        {
            if (tooltipPanel && !tooltipPanel.activeSelf)
            {
                tooltipPanel.SetActive(true);
                if (Application.isMobilePlatform)
                    PositionTooltip();
            }
        }

        private void HideTooltip()
        {
            if (tooltipPanel && tooltipPanel.activeSelf)
                tooltipPanel.SetActive(false);
        }

        private void PositionTooltip()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                RectTransform rectTransform = tooltipPanel.GetComponent<RectTransform>();
                RectTransform canvasRect = tooltipPanel.transform.root.GetComponent<Canvas>().GetComponent<RectTransform>();

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    touch.position,
                    Camera.main,
                    out Vector2 localPoint
                );

                Vector2 tooltipSize = rectTransform.sizeDelta;
                Vector2 offset = localPoint.x < canvasRect.rect.width / 2
                    ? new Vector2(tooltipSize.x / 2, 0)
                    : new Vector2(-tooltipSize.x / 2, 0);

                Vector2 anchoredPosition = localPoint + offset;
                anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, tooltipSize.x / 2, canvasRect.rect.width - tooltipSize.x / 2);
                anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, tooltipSize.y / 2, canvasRect.rect.height - tooltipSize.y / 2);

                rectTransform.anchoredPosition = anchoredPosition;
            }
        }

        private void CheckForOutsideTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
                {
                    position = touch.position
                };

                var raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, raycastResults);

                if (!raycastResults.Exists(result => result.gameObject == gameObject || result.gameObject == tooltipPanel))
                    HideTooltip();
            }
        }
    }
}
