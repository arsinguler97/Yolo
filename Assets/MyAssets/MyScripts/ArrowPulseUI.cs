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

        private Vector3 _upDefault;
        private Vector3 _downDefault;
        private Vector3 _leftDefault;
        private Vector3 _rightDefault;

        private void Awake()
        {
            _upDefault = upArrow.localScale;
            _downDefault = downArrow.localScale;
            _leftDefault = leftArrow.localScale;
            _rightDefault = rightArrow.localScale;
        }

        public void Pulse(Vector3 dir)
        {
            RectTransform arrow = null;
            Vector3 defaultScale = Vector3.one;

            if (dir == Vector3.forward) { arrow = upArrow; defaultScale = _upDefault; }
            else if (dir == Vector3.back) { arrow = downArrow; defaultScale = _downDefault; }
            else if (dir == Vector3.left) { arrow = leftArrow; defaultScale = _leftDefault; }
            else if (dir == Vector3.right) { arrow = rightArrow; defaultScale = _rightDefault; }

            if (arrow == null) return;

            arrow.DOKill(true);
            arrow.localScale = defaultScale;

            Vector3 targetScale = defaultScale * pulseScale;
            arrow.DOScale(targetScale, pulseDuration / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    arrow.DOScale(defaultScale, pulseDuration / 2)
                        .SetEase(Ease.InQuad)
                );
        }
    }
}