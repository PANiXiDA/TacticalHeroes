using Assets.Scripts.Common.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PopupManager : MonoBehaviour
    {
        public GameObject popupPanel;
        public TextMeshProUGUI popupText;
        public Button closeButton;

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

            popupPanel.SetActive(false);
            closeButton.onClick.AddListener(ClosePopup);

            TaskRunner.OnErrorOccurred += ShowPopup;
        }

        private void OnDestroy()
        {
            TaskRunner.OnErrorOccurred -= ShowPopup;
        }

        public static PopupManager Instance => _instance;

        public void ShowPopup(string message)
        {
            popupText.text = message;
            popupPanel.SetActive(true);
        }

        private void ClosePopup()
        {
            popupPanel.SetActive(false);
        }
    }
}
