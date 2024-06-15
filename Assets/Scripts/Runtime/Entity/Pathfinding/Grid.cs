using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entity.Pathfinding
{
    /// <summary>
    /// A grid class that manages and provides access to nodes.
    /// </summary>
    public class Grid : MonoBehaviour
    {
        public Tilemap groundTilemap;
        public List<Tilemap> unwalkableTilemaps;
        public int width, height;
        public LayerMask unwalkableLayer;
        public LayerMask walkableLayer;
        public float nodeRadius = 0.5f;
        public float nodeDiameter = 1.0f;

        public Vector3Int gridWorldSize;
        public Vector3Int gridPosition;
#if UNITY_EDITOR
        public bool drawGrid;
#endif

        private Node[,] _nodes;

        private void Awake()
        {
            InitializeGrid();
        }

        public void InitializeGrid()
        {
            var bounds = groundTilemap.cellBounds;
            width = bounds.size.x;
            height = bounds.size.y;

            gridWorldSize = bounds.size;
            gridPosition = bounds.position + Vector3Int.RoundToInt(transform.position);

            _nodes = new Node[gridWorldSize.x, gridWorldSize.y];
            InitializeNodes();
        }

        /// <summary>
        /// Initializes the nodes in the grid.
        /// </summary>
        private void InitializeNodes()
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var cellPosition = new Vector3Int(x, y, 0);
                    var worldPoint = groundTilemap.CellToWorld(cellPosition) + groundTilemap.tileAnchor;
                    var isWalkable = groundTilemap.HasTile(cellPosition) && !unwalkableTilemaps.Any(tilemap => tilemap.HasTile(cellPosition));
                    _nodes[x, y] = new Node(new Vector2(x, y), worldPoint, isWalkable);
                }
            }
        }

        /// <summary>
        /// Sets the nodes of the grid.
        /// </summary>
        public void InitializeNodes(Node[,] nodes) => _nodes = nodes;

        /// <summary>
        /// Gets a node at a specific position.
        /// </summary>
        /// <param name="x">The x position of the node.</param>
        /// <param name="y">The y position of the node.</param>
        /// <returns>The node at the specified position.</returns>
        public Node GetNode(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return null;
            }

            return _nodes[x, y];
        }

        /// <summary>
        /// Gets a node at a specific world position.
        /// </summary>
        /// <param name="worldPosition">The world position to get the node for.</param>
        /// <returns>The node at the specified world position.</returns>
        public Node GetNode(Vector2 worldPosition)
        {
            var percentX = Mathf.Clamp01(worldPosition.x / (width * nodeDiameter));
            var percentY = Mathf.Clamp01(worldPosition.y / (height * nodeDiameter));
            var x = Mathf.RoundToInt((width - 1) * percentX);
            var y = Mathf.RoundToInt((height - 1) * percentY);
            return _nodes[x, y];
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

                    var checkX = Mathf.RoundToInt(node.GridPosition.x) + x;
                    var checkY = Mathf.RoundToInt(node.GridPosition.y) + y;

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

        /// <summary>
        /// Sets a node at a specific position to be walkable or not.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="walkable">Whether the node is walkable or not.</param>
        public void SetWalkable(Vector2 position, bool walkable)
        {
            var node = GetNode(position);
            if (node != null)
            {
                node.isWalkable = walkable;
            }
        }
    }
}
