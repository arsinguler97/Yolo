using UnityEngine;
using DG.Tweening;

namespace MyAssets.MyScripts
{
    public class GridSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject unrevealedTilePrefab;
        [SerializeField] private GameObject safeTilePrefab;
        [SerializeField] private GameObject dangerousTilePrefab;
        [SerializeField] private GameObject unmovableTilePrefab;
        [SerializeField] private MyGameManager gameManager;

        [Header("Settings")]
        [SerializeField] private int gridSize = 7;
        [SerializeField] private float tileSpacing = 2.5f;
        [SerializeField] private float dangerousTileChance = 0.25f;

        private int _radius;
        private Vector2Int _playerPos;
        private GameObject[,] _tiles;
        private Vector3 _origin;

        private void Start()
        {
            if (gridSize % 2 == 0) gridSize++;
            _radius = gridSize / 2;
            _playerPos = Vector2Int.zero;
            _tiles = new GameObject[gridSize * 10, gridSize * 10];
            _origin = transform.position;

            SpawnInitialTiles();
            SpawnOuterRingIfMissing();
            Reveal4Neighbours();
        }

        public void MovePlayer(Vector3 dir)
        {
            Vector2Int moveDir = Vector2Int.zero;
            if (dir == Vector3.forward) moveDir = Vector2Int.up;
            else if (dir == Vector3.back) moveDir = Vector2Int.down;
            else if (dir == Vector3.left) moveDir = Vector2Int.left;
            else if (dir == Vector3.right) moveDir = Vector2Int.right;
            if (moveDir == Vector2Int.zero) return;

            Vector2Int target = _playerPos + moveDir;
            GameObject targetTile = GetTile(target);
            if (targetTile == null) return;

            if (targetTile.CompareTag("UnmovableTile")) return;

            if (targetTile.CompareTag("DangerousTile"))
            {
                gameManager.OnDangerTile();
                return;
            }

            if (targetTile.CompareTag("SafeTile"))
                gameManager.OnSafeTile();

            ConvertToUnmovable(_playerPos);
            _playerPos = target;

            Vector3 shift = new Vector3(-moveDir.x * tileSpacing, 0f, -moveDir.y * tileSpacing);
            
            foreach (Transform child in transform)
            {
                Vector3 targetPos = child.position + shift;
                child.DOMove(targetPos, 0.3f).SetEase(Ease.InOutSine);
            }

            SpawnOuterRingIfMissing();
            Reveal4Neighbours();
        }

        private void SpawnInitialTiles()
        {
            for (int dx = -_radius; dx <= _radius; dx++)
            {
                for (int dy = -_radius; dy <= _radius; dy++)
                {
                    Vector2Int g = _playerPos + new Vector2Int(dx, dy);
                    SpawnIfMissing(g, unrevealedTilePrefab, "UnrevealedTile");
                }
            }
            ReplaceTile(_playerPos, safeTilePrefab, "SafeTile");
        }

        private void SpawnOuterRingIfMissing()
        {
            int left = _playerPos.x - _radius;
            int right = _playerPos.x + _radius;
            int down = _playerPos.y - _radius;
            int up = _playerPos.y + _radius;

            for (int x = left; x <= right; x++)
            {
                SpawnIfMissing(new Vector2Int(x, up), unrevealedTilePrefab, "UnrevealedTile");
                SpawnIfMissing(new Vector2Int(x, down), unrevealedTilePrefab, "UnrevealedTile");
            }
            for (int y = down + 1; y <= up - 1; y++)
            {
                SpawnIfMissing(new Vector2Int(left, y), unrevealedTilePrefab, "UnrevealedTile");
                SpawnIfMissing(new Vector2Int(right, y), unrevealedTilePrefab, "UnrevealedTile");
            }
        }

        private void Reveal4Neighbours()
        {
            Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            bool safeSpawned = false;
            Vector2Int lastUnrevealed = Vector2Int.zero;

            foreach (var d in dirs)
            {
                Vector2Int p = _playerPos + d;
                GameObject t = GetTile(p);
                if (t == null) continue;
                if (t.CompareTag("UnrevealedTile"))
                {
                    lastUnrevealed = p;
                    bool danger = Random.value < dangerousTileChance;
                    if (!danger) safeSpawned = true;

                    ReplaceTile(p,
                        danger ? dangerousTilePrefab : safeTilePrefab,
                        danger ? "DangerousTile" : "SafeTile");
                }
            }

            if (!safeSpawned && lastUnrevealed != Vector2Int.zero)
            {
                ReplaceTile(lastUnrevealed, safeTilePrefab, "SafeTile");
                Debug.Log("[Safety Adjust] Forced one Safe tile at " + lastUnrevealed);
            }
        }

        private void ConvertToUnmovable(Vector2Int p)
        {
            ReplaceTile(p, unmovableTilePrefab, "UnmovableTile");
        }

        private void SpawnIfMissing(Vector2Int p, GameObject prefab, string tileTag)
        {
            if (GetTile(p) != null) return;
            Vector3 wp = GridToWorld(p);
            GameObject go = Instantiate(prefab, wp, Quaternion.identity, transform);
            go.tag = tileTag;
            SetTile(p, go);
        }

        private void ReplaceTile(Vector2Int p, GameObject prefab, string tileTag)
        {
            GameObject old = GetTile(p);
            if (old != null) Destroy(old);
            Vector3 wp = GridToWorld(p);
            GameObject go = Instantiate(prefab, wp, Quaternion.identity, transform);
            go.tag = tileTag;
            SetTile(p, go);
        }

        private Vector3 GridToWorld(Vector2Int gp)
        {
            Vector2Int rel = gp - _playerPos;
            return _origin + new Vector3(rel.x * tileSpacing, 0f, rel.y * tileSpacing);
        }

        private GameObject GetTile(Vector2Int gp)
        {
            int w = _tiles.GetLength(0);
            int h = _tiles.GetLength(1);
            int ix = gp.x + w / 2;
            int iy = gp.y + h / 2;
            if (ix < 0 || iy < 0 || ix >= w || iy >= h) return null;
            return _tiles[ix, iy];
        }

        private void SetTile(Vector2Int gp, GameObject go)
        {
            int w = _tiles.GetLength(0);
            int h = _tiles.GetLength(1);
            int ix = gp.x + w / 2;
            int iy = gp.y + h / 2;
            if (ix < 0 || iy < 0 || ix >= w || iy >= h) return;
            _tiles[ix, iy] = go;
        }
    }
}
