using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entity.Pathfinding
{
    /// <summary>
    /// A grid class that manages and provides access to nodes.
    /// </summary>
    public class PathfindingGrid : MonoBehaviour
    {
        public Grid tileGrid;
        public Tilemap groundTilemap;
        public List<Tilemap> unwalkableTilemaps;
        public int width, height;
        public float nodeRadius = 0.5f;
        public float nodeDiameter = 1.0f;

        public Vector3Int gridWorldSize;
        public Vector3Int gridPosition;

#if UNITY_EDITOR
        public bool drawGrid;
#endif

        private Vector2Int _offset, _origin;

        private Node[,] _nodes;

        private void Awake()
        {
            InitializeGrid();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!drawGrid || _nodes == null) return;

            foreach (var node in _nodes)
            {
                if (node != null && node.isWalkable)
                {
                    var pos = node.WorldPosition;
                    var vec3 = new Vector3(pos.x, pos.y, 0);
                    Gizmos.DrawCube(vec3, Vector3.one * .1f);
                }
            }
        }
#endif

        public void InitializeGrid()
        {
            var bounds = groundTilemap.cellBounds;

            // Store variables required for calculating grid position.
            _offset = new Vector2Int(
                (int) (1 / tileGrid.cellSize.x),
                (int) (1 / tileGrid.cellSize.y));
            // gridPosition (absolute) * offset == integer number
            _origin = bounds.min.ToVector2();

            width = bounds.size.x * _offset.x;
            height = bounds.size.y * _offset.y;

            _nodes = new Node[width, height];
            InitializeNodes();
        }

        /// <summary>
        /// Converts a world position to a pathfinder grid position.
        /// </summary>
        public Vector2Int ToGridPosition(Vector2 worldPosition)
        {
            // Determine the tile position from the world position.
            var tilePosition = groundTilemap.WorldToCell(worldPosition);
            return ToGridPosition(tilePosition.ToVector2());
        }

        /// <summary>
        /// Converts the position of a tile/cell to a pathfinder grid position.
        /// </summary>
        public Vector2Int ToGridPosition(Vector2Int cellPosition)
            => ToGridPosition(cellPosition.x, cellPosition.y);

        /// <summary>
        /// Converts the position of a tile/cell to a pathfinder grid position.
        /// </summary>
        public Vector2Int ToGridPosition(int x, int y)
            => new Vector2Int(x - _origin.x, y - _origin.y);

        /// <summary>
        /// Initializes the nodes in the grid.
        /// </summary>
        private void InitializeNodes()
        {
            var bounds = groundTilemap.cellBounds;

            // Get all tilemaps.
            var tileMaps = FindObjectsOfType<Tilemap>();

            for (var x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (var y = bounds.min.y; y < bounds.max.y; y++)
                {
                    var cellPosition = new Vector3Int(x, y, 0);

                    // Check if the cell is a real cell.
                    // This fails if no tile is present on any layer.
                    if (!tileMaps.Any(tm => tm.HasTile(cellPosition))) continue;

                    // "Normalize" our position to anchor to the bottom left corner.
                    var gridLocalPos = ToGridPosition(x, y);

                    // Determine if the cell is walkable at all.
                    var isWalkable = !unwalkableTilemaps.Any(tilemap => tilemap.HasTile(cellPosition));
                    // Determine the real world position of the cell.
                    var worldPoint = groundTilemap.CellToWorld(cellPosition) + groundTilemap.tileAnchor;
                    // Create our node.
                    _nodes[gridLocalPos.x, gridLocalPos.y] =
                        new Node(gridLocalPos, worldPoint, isWalkable);
                }
            }
        }

        /// <summary>
        /// Gets a node at a specific position.
        /// </summary>
        /// <param name="x">The x position of the node.</param>
        /// <param name="y">The y position of the node.</param>
        /// <returns>The node at the specified position.</returns>
        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return null;

            return _nodes[x, y];
        }

        /// <summary>
        /// Gets a node at a specific world position.
        /// </summary>
        /// <param name="worldPosition">The world position to get the node for.</param>
        /// <returns>The node at the specified world position.</returns>
        public Node GetNode(Vector2 worldPosition)
        {
            var position = ToGridPosition(worldPosition);
            var isWithinBounds = position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
            return isWithinBounds ? _nodes[position.x, position.y] : null;
        }

        /// <summary>
        /// Finds the neighbors of a given node.
        /// </summary>
        /// <param name="node">The node to find neighbors for.</param>
        /// <returns>A list of neighboring nodes.</returns>
        public IEnumerable<Node> FindNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    // Skip the center node.
                    if (x == 0 && y == 0) continue;

                    var checkX = node.GridPosition.x + x;
                    var checkY = node.GridPosition.y + y;

                    var neighbor = GetNode(checkX, checkY);
                    if (neighbor != null)
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Checks if a node is walkable.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        public bool IsWalkable(Vector2 position)
        {
            var node = GetNode(position);
            return node != null && node.isWalkable;
        }
    }
}
