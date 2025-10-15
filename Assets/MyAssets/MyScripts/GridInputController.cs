using UnityEngine;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.Input;

namespace MyAssets.MyScripts
{
    /// <summary>
    /// Listens to MoveInputEvent from the InputService (instead of using Input.GetKeyDown).
    /// Converts input into grid-based moves for GridSpawner.
    /// </summary>
    public class GridInputController : MonoBehaviour
    {
        [SerializeField] private GridSpawner spawner;

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
                spawner.MovePlayer(Vector3.forward);
            else if (input.y < -0.5f)
                spawner.MovePlayer(Vector3.back);
            else if (input.x < -0.5f)
                spawner.MovePlayer(Vector3.left);
            else if (input.x > 0.5f)
                spawner.MovePlayer(Vector3.right);
        }
    }
}