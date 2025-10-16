using UnityEngine;
using System.Collections.Generic;
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
        [SerializeField] private float moveCooldown = 0.35f;

        private int _radius;
        private Vector2Int _playerPos;
        private Dictionary<Vector2Int, GameObject> _tiles = new();
        private bool _canMove = true;

        private void Start()
        {
            if (gridSize % 2 == 0) gridSize++;
            _radius = gridSize / 2;
            _playerPos = Vector2Int.zero;

            SpawnInitialTiles();
            SpawnOuterRingIfMissing();
            Reveal4Neighbours();
        }

        public void MovePlayer(Vector3 dir)
        {
            if (!_canMove) return; // anti-spam
            _canMove = false;
            Invoke(nameof(ResetMove), moveCooldown);

            Vector2Int moveDir = Vector2Int.zero;
            if (dir == Vector3.forward) moveDir = Vector2Int.up;
            else if (dir == Vector3.back) moveDir = Vector2Int.down;
            else if (dir == Vector3.left) moveDir = Vector2Int.left;
            else if (dir == Vector3.right) moveDir = Vector2Int.right;
            if (moveDir == Vector2Int.zero) return;

            Vector2Int target = _playerPos + moveDir;
            if (!_tiles.TryGetValue(target, out var targetTile)) return;

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

        private void ResetMove()
        {
            _canMove = true;
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
                if (!_tiles.TryGetValue(p, out var t)) continue;
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
            }
        }

        private void ConvertToUnmovable(Vector2Int p)
        {
            ReplaceTile(p, unmovableTilePrefab, "UnmovableTile");
        }

        private void SpawnIfMissing(Vector2Int p, GameObject prefab, string tileType)
        {
            if (_tiles.ContainsKey(p)) return;
            var go = Instantiate(prefab, GridToWorld(p), Quaternion.identity, transform);
            go.tag = tileType;
            _tiles[p] = go;
        }

        private void ReplaceTile(Vector2Int p, GameObject prefab, string tileType)
        {
            if (_tiles.TryGetValue(p, out var old)) Destroy(old);
            var go = Instantiate(prefab, GridToWorld(p), Quaternion.identity, transform);
            go.tag = tileType;
            _tiles[p] = go;
        }

        private Vector3 GridToWorld(Vector2Int gp)
        {
            return transform.position + new Vector3(
                (gp.x - _playerPos.x) * tileSpacing,
                0f,
                (gp.y - _playerPos.y) * tileSpacing);
        }
    }
}
