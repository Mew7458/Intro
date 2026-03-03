using System.Collections.Generic;
using UnityEngine;

namespace Intro.UnityPrototype.Core
{
    public class GridBoard : MonoBehaviour
    {
        [Header("Board Size")]
        [SerializeField] private int rows = 9;
        [SerializeField] private int cols = 26;

        [Header("Render")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private Transform cellRoot;
        [SerializeField] private GameObject cellPrefab;

        private readonly HashSet<Vector2Int> blockedCells = new();

        public int Rows => rows;
        public int Cols => cols;

        public bool InBounds(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < rows && cell.y >= 0 && cell.y < cols;
        }

        public bool IsWalkable(Vector2Int cell)
        {
            return InBounds(cell) && !blockedCells.Contains(cell);
        }

        public void SetBlocked(Vector2Int cell, bool blocked)
        {
            if (!InBounds(cell)) return;

            if (blocked) blockedCells.Add(cell);
            else blockedCells.Remove(cell);
        }

        public int Manhattan(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public Vector3 GridToWorld(Vector2Int cell)
        {
            return new Vector3(cell.y * cellSize, 0f, -cell.x * cellSize);
        }

        public IEnumerable<Vector2Int> GetAdjacent(Vector2Int cell)
        {
            var dirs = new[]
            {
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(0, 1),
            };

            foreach (var dir in dirs)
            {
                var next = cell + dir;
                if (IsWalkable(next)) yield return next;
            }
        }

        [ContextMenu("Generate Debug Cells")]
        public void GenerateDebugCells()
        {
            if (cellPrefab == null || cellRoot == null)
            {
                Debug.LogWarning("GridBoard: 缺少 cellPrefab 或 cellRoot。", this);
                return;
            }

            for (var i = cellRoot.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(cellRoot.GetChild(i).gameObject);
            }

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var obj = Instantiate(cellPrefab, cellRoot);
                    obj.transform.position = GridToWorld(new Vector2Int(r, c));
                    obj.name = $"Cell_{r}_{c}";
                }
            }
        }
    }
}
