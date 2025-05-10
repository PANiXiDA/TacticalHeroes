using DG.Tweening;
using TMPro;

using UnityEngine;

namespace Assets.Scripts.UI.Battle.Views
{
    public sealed class DamagePopupView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _damageText;
        [SerializeField] private TextMeshProUGUI _deathsText;

        private CanvasGroup _group;
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            CacheComponents();
        }

        private void CacheComponents()
        {
            _group = GetComponent<CanvasGroup>();
            _rectTransform = (RectTransform)transform;
        }

        public void Init(Vector2 position, int damage, int deaths)
        {
            _rectTransform.anchoredPosition = position;
            _damageText.text = damage.ToString();
            _deathsText.text = deaths.ToString();

            PlayAnimation();
        }

        private void PlayAnimation()
        {
            _group.alpha = 1f;

            DOTween.Sequence()
                .Append(_rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 80f, 5f)
                .SetEase(Ease.OutQuad))
                .Join(_group.DOFade(0f, 5f))
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
