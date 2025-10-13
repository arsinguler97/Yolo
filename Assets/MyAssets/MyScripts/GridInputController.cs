using UnityEngine;

namespace MyAssets.MyScripts
{
    public class GridInputController : MonoBehaviour
    {
        [SerializeField] private GridSpawner spawner;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
                spawner.MovePlayer(Vector3.forward);
            else if (Input.GetKeyDown(KeyCode.S))
                spawner.MovePlayer(Vector3.back);
            else if (Input.GetKeyDown(KeyCode.A))
                spawner.MovePlayer(Vector3.left);
            else if (Input.GetKeyDown(KeyCode.D))
                spawner.MovePlayer(Vector3.right);
        }
    }
}