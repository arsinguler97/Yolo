using UnityEngine;

namespace MyAssets.MyScripts
{
    /// <summary>
    /// Generates tiles (safe or dangerous) around the player.
    /// - Player stays fixed at the center.
    /// - Four tiles (up, down, left, right) spawn around the player.
    /// - The chance of spawning a dangerous tile increases with time.
    /// - At least one safe tile is always guaranteed.
    /// </summary>
    public class GridSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject safeTilePrefab;
        [SerializeField] private GameObject dangerousTilePrefab;
        [SerializeField] private MyGameManager gameManager;

        [Header("Grid Settings")]
        [SerializeField] private float tileSpacing = 2.5f;
        [SerializeField] private float initialDangerChance = 0.25f;
        [SerializeField] private float maxDangerChance = 0.75f;
        [SerializeField] private float collisionRadius = 0.5f;

        [Header("Timer Settings")]
        [SerializeField] private float gameDuration = 60f; // Duration in seconds

        private float _elapsedTime;
        private GameObject[] _tiles = new GameObject[5];
        private Collider[] _hits = new Collider[10];

        private readonly Vector3[] _directions =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        private void Start()
        {
            SpawnTiles();
        }

        /// <summary>
        /// Called from GridInputController when the player "moves".
        /// Actually the player stays still; only tiles refresh.
        /// </summary>
        public void MovePlayer(Vector3 direction)
        {
            _elapsedTime += Time.deltaTime; // danger chance grows with time
            player.rotation = Quaternion.LookRotation(direction);

            CheckTileAtDirection(direction);
            SpawnTiles();
        }

        /// <summary>
        /// Checks what kind of tile exists in the movement direction.
        /// </summary>
        private void CheckTileAtDirection(Vector3 direction)
        {
            Vector3 targetPos = player.position + direction * tileSpacing;
            int hitCount = Physics.OverlapSphereNonAlloc(targetPos, collisionRadius, _hits);

            for (int i = 0; i < hitCount; i++)
            {
                var col = _hits[i];
                if (col.CompareTag("DangerousTile"))
                {
                    Debug.Log("Stepped on DangerousTile!");
                    gameManager.OnDangerTile();
                    return;
                }
                else if (col.CompareTag("SafeTile"))
                {
                    Debug.Log("Stepped on SafeTile!");
                    gameManager.OnSafeTile();
                    return;
                }
            }
        }

        /// <summary>
        /// Spawns 5 tiles:
        /// 1 center (always safe)
        /// + 4 sides (at least one must be safe)
        /// </summary>
        private void SpawnTiles()
        {
            // Destroy previous tiles
            foreach (var t in _tiles)
                if (t != null)
                    Destroy(t);

            // Compute current danger chance (increases over time)
            float tRatio = Mathf.Clamp01(_elapsedTime / gameDuration);
            float dangerChance = Mathf.Lerp(initialDangerChance, maxDangerChance, tRatio);

            // Always spawn safe tile under the player
            Vector3 centerPos = player.position;
            centerPos.y = 0.1f;
            _tiles[0] = Instantiate(safeTilePrefab, centerPos, Quaternion.identity);
            _tiles[0].tag = "SafeTile";

            bool hasSafeTile = false;

            // Spawn 4 directional tiles
            for (int i = 0; i < _directions.Length; i++)
            {
                bool isDangerous = Random.value < dangerChance;
                GameObject prefab = isDangerous ? dangerousTilePrefab : safeTilePrefab;

                if (!isDangerous)
                    hasSafeTile = true;

                Vector3 pos = player.position + _directions[i] * tileSpacing;
                pos.y = 0.1f;
                _tiles[i + 1] = Instantiate(prefab, pos, Quaternion.identity);
                _tiles[i + 1].tag = isDangerous ? "DangerousTile" : "SafeTile";
            }

            // Guarantee at least one safe tile around
            if (!hasSafeTile)
            {
                int randomIndex = Random.Range(0, _directions.Length);
                Vector3 pos = player.position + _directions[randomIndex] * tileSpacing;
                pos.y = 0.1f;

                if (_tiles[randomIndex + 1] != null)
                    Destroy(_tiles[randomIndex + 1]);

                _tiles[randomIndex + 1] = Instantiate(safeTilePrefab, pos, Quaternion.identity);
                _tiles[randomIndex + 1].tag = "SafeTile";
            }
        }
    }
}
