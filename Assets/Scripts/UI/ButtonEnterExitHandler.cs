using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class ButtonEnterExitHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject _descriptionObject;

        private void Awake()
        {
            var childTransform = GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(t => t.name == "Decription");
            if (childTransform != null)
            {
                _descriptionObject = childTransform.gameObject;
            }
            else
            {
                Debug.LogWarning("Decription object not found.");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_descriptionObject != null)
            {
                _descriptionObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_descriptionObject != null)
            {
                _descriptionObject.SetActive(false);
            }
        }
    }
}
