using UnityEngine;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Input;

namespace MyAssets.MyScripts
{
    /// <summary>
    /// Listens to MoveInputEvent from the InputService (EventBus-based input system).
    /// Sends direction info to GridSpawner and triggers animation on SpriteAnimatorSimple.
    /// </summary>
    public class GridInputController : MonoBehaviour
    {
        [SerializeField] private GridSpawner spawner;
        [SerializeField] private SpriteAnimatorSimple animatorSimple;

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<MoveInputEvent>(this, OnMoveInput);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<MoveInputEvent>(this);
        }

        private void OnMoveInput(MoveInputEvent evt)
        {
            Vector2 input = evt.Input;

            if (input.y > 0.5f)
            {
                animatorSimple.PlayMoveAnimation(Vector3.forward);
                spawner.MovePlayer(Vector3.forward);
            }
            else if (input.y < -0.5f)
            {
                animatorSimple.PlayMoveAnimation(Vector3.back);
                spawner.MovePlayer(Vector3.back);
            }
            else if (input.x < -0.5f)
            {
                animatorSimple.PlayMoveAnimation(Vector3.left);
                spawner.MovePlayer(Vector3.left);
            }
            else if (input.x > 0.5f)
            {
                animatorSimple.PlayMoveAnimation(Vector3.right);
                spawner.MovePlayer(Vector3.right);
            }
        }
    }
}