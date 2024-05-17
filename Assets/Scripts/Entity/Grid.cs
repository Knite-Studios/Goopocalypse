using System.Collections.Generic;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// A grid class that manages and provides access to nodes.
    /// </summary>
    public class Grid
    {
        private readonly Node[,] _nodes;
        private readonly int _width, _height;
        
        public Grid(int width, int height)
        {
            _width = width;
            _height = height;
            _nodes = new Node[width, height];
            InitializeNodes();
        }
        
        /// <summary>
        /// Initializes the nodes in the grid.
        /// </summary>
        private void InitializeNodes()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    _nodes[x, y] = new Node(new Vector2(x, y), true);
                }
            }
        }
        
        private Node GetNode(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return null;
            }
            
            return _nodes[x, y];
        }
        
        /// <summary>
        /// Computes the nearby nodes of a given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IEnumerable<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
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