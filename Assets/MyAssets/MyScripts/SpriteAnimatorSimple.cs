using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace MyAssets.MyScripts
{
    [System.Serializable]
    public struct SpriteSet
    {
        public Sprite idleUp, idleDown, idleLeft, idleRight;
        public Sprite[] walkUp, walkDown, walkLeft, walkRight;
    }

    public class SpriteAnimatorSimple : MonoBehaviour
    {
        [Header("Renderer Reference")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Sprite Sets (Evolution Stages)")]
        [SerializeField] private SpriteSet infantSet, childSet, teenagerSet, adultSet, elderSet;

        [SerializeField] private float frameDuration = 0.15f;

        private SpriteSet _currentSet;
        private Coroutine _walkRoutine;
        private string _lastDirection = "Down";
        private Vector3 _defaultScale;

        private void Start()
        {
            _currentSet = infantSet;
            _defaultScale = transform.localScale;
        }

        public void SetSpriteSet(string stage)
        {
            switch (stage)
            {
                case "Infant": _currentSet = infantSet; break;
                case "Child": _currentSet = childSet; break;
                case "Teenager": _currentSet = teenagerSet; break;
                case "Adult": _currentSet = adultSet; break;
                case "Elder": _currentSet = elderSet; break;
            }
        }

        public void PlayMoveAnimation(Vector3 dir)
        {
            transform.DOKill(true);
            transform.localScale = _defaultScale;

            Vector3 bigger = _defaultScale * 1.15f;
            transform.DOScale(bigger, 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(() => transform.DOScale(_defaultScale, 0.1f).SetEase(Ease.InQuad));

            if (_walkRoutine != null)
                StopCoroutine(_walkRoutine);

            if (dir == Vector3.forward)
                _walkRoutine = StartCoroutine(PlayWalk(_currentSet.walkUp, _currentSet.idleUp, "Up"));
            else if (dir == Vector3.back)
                _walkRoutine = StartCoroutine(PlayWalk(_currentSet.walkDown, _currentSet.idleDown, "Down"));
            else if (dir == Vector3.left)
                _walkRoutine = StartCoroutine(PlayWalk(_currentSet.walkLeft, _currentSet.idleLeft, "Left"));
            else if (dir == Vector3.right)
                _walkRoutine = StartCoroutine(PlayWalk(_currentSet.walkRight, _currentSet.idleRight, "Right"));
            else
                SetIdleSprite();
        }

        private IEnumerator PlayWalk(Sprite[] frames, Sprite idleSprite, string dir)
        {
            _lastDirection = dir;
            foreach (var frame in frames)
            {
                spriteRenderer.sprite = frame;
                yield return new WaitForSeconds(frameDuration);
            }
            spriteRenderer.sprite = idleSprite;
            _walkRoutine = null;
        }

        private void SetIdleSprite()
        {
            switch (_lastDirection)
            {
                case "Up": spriteRenderer.sprite = _currentSet.idleUp; break;
                case "Down": spriteRenderer.sprite = _currentSet.idleDown; break;
                case "Left": spriteRenderer.sprite = _currentSet.idleLeft; break;
                case "Right": spriteRenderer.sprite = _currentSet.idleRight; break;
            }
        }
    }
}
