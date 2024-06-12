using System.Collections.Generic;
using UnityEngine;

namespace Entity.Pathfinding
{
    /// <summary>
    /// A grid class that manages and provides access to nodes.
    /// </summary>
    public class Grid : MonoBehaviour
    {
        public int width, height;
        public LayerMask unwalkableLayer;
        public LayerMask walkableLayer;
        public float nodeRadius = 0.5f;
        public float nodeDiameter = 1.0f;

#if UNITY_EDITOR
        public bool drawGrid;
#endif

        private Node[,] _nodes;

        private void Awake()
        {
            InitializeGrid(width, height, unwalkableLayer, walkableLayer, nodeRadius);
        }

        /// <summary>
        /// This creates a grid from scratch.
        /// Usually only used for testing.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="unwalkableLayerMask"></param>
        /// <param name="walkableLayerMask"></param>
        /// <param name="nodeRadius"></param>
        public void InitializeGrid(
            int width,
            int height,
            LayerMask unwalkableLayerMask,
            LayerMask walkableLayerMask,
            float nodeRadius)
        {
            this.width = width;
            this.height = height;
            unwalkableLayer = unwalkableLayerMask;
            walkableLayer = walkableLayerMask;
            this.nodeRadius = nodeRadius;
            // nodeDiameter = nodeRadius * 2;
            _nodes = new Node[width, height];
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
                    var worldPoint = new Vector2(x * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius);
                    var isWalkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableLayer);
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

                    var checkX = Mathf.RoundToInt(node.gridPosition.x) + x;
                    var checkY = Mathf.RoundToInt(node.gridPosition.y) + y;

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
