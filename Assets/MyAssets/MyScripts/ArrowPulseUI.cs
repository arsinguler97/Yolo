using UnityEngine;
using DG.Tweening;

namespace MyAssets.MyScripts
{
    public class ArrowPulseUI : MonoBehaviour
    {
        [Header("Arrow References")]
        [SerializeField] private RectTransform upArrow;
        [SerializeField] private RectTransform downArrow;
        [SerializeField] private RectTransform leftArrow;
        [SerializeField] private RectTransform rightArrow;

        [SerializeField] private float pulseScale = 1.5f;
        [SerializeField] private float pulseDuration = 0.2f;

        public void Pulse(Vector3 dir)
        {
            RectTransform arrow = null;

            if (dir == Vector3.forward) arrow = upArrow;
            else if (dir == Vector3.back) arrow = downArrow;
            else if (dir == Vector3.left) arrow = leftArrow;
            else if (dir == Vector3.right) arrow = rightArrow;

            if (arrow == null) return;

            arrow.DOKill();
            Vector3 originalScale = arrow.localScale;
            Vector3 targetScale = originalScale * pulseScale;

            arrow.DOScale(targetScale, pulseDuration / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    arrow.DOScale(originalScale, pulseDuration / 2)
                        .SetEase(Ease.InQuad)
                );
        }
    }
}