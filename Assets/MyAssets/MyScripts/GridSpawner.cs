using UnityEngine;

namespace MyAssets.MyScripts
{
    /// <summary>
    /// Spawns safe/dangerous tiles around the player.
    /// - Player stays centered.
    /// - One tile in each direction.
    /// - At least one safe tile guaranteed.
    /// - Danger chance increases over time.
    /// </summary>
    public class GridSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject safeTilePrefab;
        [SerializeField] private GameObject dangerousTilePrefab;
        [SerializeField] private MyGameManager gameManager;
        [SerializeField] private SpriteAnimatorSimple spriteAnimator;

        [Header("Grid Settings")]
        [SerializeField] private float tileSpacing = 2.5f;
        [SerializeField] private float initialDangerChance = 0.25f;
        [SerializeField] private float maxDangerChance = 0.75f;
        [SerializeField] private float collisionRadius = 0.5f;

        [Header("Timer Settings")]
        [SerializeField] private float gameDuration = 60f;

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

        public void MovePlayer(Vector3 direction)
        {
            // 🔹 Directional sprite animation
            spriteAnimator?.PlayMoveAnimation(direction);

            _elapsedTime += Time.deltaTime;
            CheckTileAtDirection(direction);
            SpawnTiles();
        }

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

        private void SpawnTiles()
        {
            foreach (var t in _tiles)
                if (t != null)
                    Destroy(t);

            float tRatio = Mathf.Clamp01(_elapsedTime / gameDuration);
            float dangerChance = Mathf.Lerp(initialDangerChance, maxDangerChance, tRatio);

            // Always spawn a safe tile under the player
            Vector3 centerPos = player.position;
            centerPos.y = 0.1f;
            _tiles[0] = Instantiate(safeTilePrefab, centerPos, Quaternion.identity);
            _tiles[0].tag = "SafeTile";

            bool hasSafeTile = false;

            // Spawn tiles in each direction
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

            // Guarantee at least one safe tile
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
