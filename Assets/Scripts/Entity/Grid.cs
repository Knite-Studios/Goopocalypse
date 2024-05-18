using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entity
{
    /// <summary>
    /// A grid class that manages and provides access to nodes.
    /// </summary>
    public class Grid : MonoBehaviour
    {
        public LayerMask unwalkableLayer;
        public LayerMask walkableLayer;
        public float nodeRadius;
        public int width, height;
        public float nodeDiameter;
        
        private Node[,] _nodes;

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
            nodeDiameter = nodeRadius * 2;
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
                    var worldPoint = new Vector3(x * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius, 0);
                    // var isWalkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableLayer) &&
                    //                    Physics2D.OverlapCircle(worldPoint, nodeRadius, walkableLayer));
                    var isWalkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableLayer);
                    _nodes[x, y] = new Node(new Vector2Int(x, y), isWalkable);
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
            {
                return null;
            }
            
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

                    var checkX = node.X + x;
                    var checkY = node.Y + y;

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
        /// Sets a node at a specific position to be walkable or not.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="walkable">Whether the node is walkable or not.</param>
        public void SetWalkable(Vector2 position, bool walkable)
        {
            var node = GetNode((int) position.x, (int) position.y);
            if (node != null)
            {
                node.isWalkable = walkable;
            }
        }
    }
}