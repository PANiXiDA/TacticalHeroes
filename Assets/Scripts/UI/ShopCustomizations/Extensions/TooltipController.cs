using UnityEngine.EventSystems;
using UnityEngine;

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
            if (isPressing)
            {
                pressTime += Time.deltaTime;
                if (pressTime >= longPressDuration)
                {
                    ShowTooltip();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Application.isMobilePlatform)
            {
                ShowTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Application.isMobilePlatform)
            {
                HideTooltip();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressing = true;
            pressTime = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
            pressTime = 0f;
            HideTooltip();
        }

        private void ShowTooltip()
        {
            if (tooltipPanel != null && !tooltipPanel.activeSelf)
            {
                tooltipPanel.SetActive(true);
            }
        }

        private void HideTooltip()
        {
            if (tooltipPanel != null && tooltipPanel.activeSelf)
            {
                tooltipPanel.SetActive(false);
            }
        }
    }
}
