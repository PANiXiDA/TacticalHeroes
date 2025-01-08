using Assets.Scripts.Common.Helpers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _scroll;

        [SerializeField]
        private GameObject _popupPanel;

        [SerializeField]
        public TextMeshProUGUI _popupText;

        [SerializeField]
        public Button _closeButton;

        private static PopupManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            DontDestroyOnLoad(gameObject);

            _popupPanel.SetActive(false);
            _closeButton.onClick.AddListener(ClosePopup);

            TaskRunner.OnErrorOccurred += ShowPopup;
        }

        private void OnDestroy()
        {
            TaskRunner.OnErrorOccurred -= ShowPopup;
        }

        public static PopupManager Instance => _instance;

        public void ShowPopup(string message)
        {
            _popupText.text = message;
            _popupPanel.SetActive(true);

            _scroll.sizeDelta = new Vector2(_scroll.sizeDelta.x, 0);
            _scroll.DOSizeDelta(new Vector2(_scroll.sizeDelta.x, 1000), 1f).SetEase(Ease.InBack);
        }

        private void ClosePopup()
        {
            _scroll.DOSizeDelta(new Vector2(_scroll.sizeDelta.x, 0), 1f).SetEase(Ease.InBack).OnComplete(() => _popupPanel.SetActive(false));
        }
    }
}
