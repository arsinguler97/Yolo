using UnityEngine;
using DG.Tweening;

namespace MyAssets.MyScripts
{
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
                    gameManager.OnDangerTile();
                    return;
                }
                else if (col.CompareTag("SafeTile"))
                {
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
            bool hasSafeTile = false;

            Vector3 centerPos = player.position;
            centerPos.y = 0f;
            _tiles[0] = Instantiate(safeTilePrefab, centerPos, Quaternion.identity);
            _tiles[0].tag = "SafeTile";

            for (int i = 0; i < _directions.Length; i++)
            {
                bool isDangerous = Random.value < dangerChance;
                GameObject prefab = isDangerous ? dangerousTilePrefab : safeTilePrefab;
                if (!isDangerous) hasSafeTile = true;

                Vector3 targetPos = player.position + _directions[i] * tileSpacing;
                targetPos.y = 0f;
                Vector3 startPos = targetPos + (-_directions[i]) * tileSpacing * 1.5f;

                _tiles[i + 1] = Instantiate(prefab, startPos, Quaternion.identity);
                _tiles[i + 1].tag = isDangerous ? "DangerousTile" : "SafeTile";
                _tiles[i + 1].transform.DOMove(targetPos, 0.35f).SetEase(Ease.OutCubic);
            }

            if (!hasSafeTile)
            {
                int randomIndex = Random.Range(0, _directions.Length);
                Vector3 pos = player.position + _directions[randomIndex] * tileSpacing;
                pos.y = 0f;

                if (_tiles[randomIndex + 1] != null)
                    Destroy(_tiles[randomIndex + 1]);

                _tiles[randomIndex + 1] = Instantiate(safeTilePrefab, pos, Quaternion.identity);
                _tiles[randomIndex + 1].tag = "SafeTile";
            }
        }
    }
}
