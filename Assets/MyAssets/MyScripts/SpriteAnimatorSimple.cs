using UnityEngine;
using System.Collections;

namespace MyAssets.MyScripts
{
    public class SpriteAnimatorSimple : MonoBehaviour
    {
        [Header("Renderer Reference")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Idle Sprites")]
        [SerializeField] private Sprite idleUp;
        [SerializeField] private Sprite idleDown;
        [SerializeField] private Sprite idleLeft;
        [SerializeField] private Sprite idleRight;

        [Header("Walk Animations (2 frames each)")]
        [SerializeField] private Sprite[] walkUp;
        [SerializeField] private Sprite[] walkDown;
        [SerializeField] private Sprite[] walkLeft;
        [SerializeField] private Sprite[] walkRight;

        [SerializeField] private float frameDuration = 0.15f;

        private Coroutine _walkRoutine;
        private string _lastDirection = "Down";

        public void PlayMoveAnimation(Vector3 dir)
        {
            if (_walkRoutine != null)
                StopCoroutine(_walkRoutine);

            if (dir == Vector3.forward)
                _walkRoutine = StartCoroutine(PlayWalk(walkUp, idleUp, "Up"));
            else if (dir == Vector3.back)
                _walkRoutine = StartCoroutine(PlayWalk(walkDown, idleDown, "Down"));
            else if (dir == Vector3.left)
                _walkRoutine = StartCoroutine(PlayWalk(walkLeft, idleLeft, "Left"));
            else if (dir == Vector3.right)
                _walkRoutine = StartCoroutine(PlayWalk(walkRight, idleRight, "Right"));
            else
                SetIdleSprite(); // fallback
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
                case "Up": spriteRenderer.sprite = idleUp; break;
                case "Down": spriteRenderer.sprite = idleDown; break;
                case "Left": spriteRenderer.sprite = idleLeft; break;
                case "Right": spriteRenderer.sprite = idleRight; break;
            }
        }
    }
}
