using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class Node
    {
        [CanBeNull] public Node parent;
        public readonly Vector2 position;
        public bool isWalkable;
        public int gCost, hCost;
        
        public int FCost => gCost + hCost;

        public int X => (int) position.x;
        public int Y => (int) position.y;
        
        public Node(Vector2 position, bool isWalkable)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }

        /// <summary>
        /// Gets the distance to another node.
        /// </summary>
        public int GetDistanceTo(Node other)
        {
            var distanceX = Mathf.Abs(X - other.X);
            var distanceY = Mathf.Abs(Y - other.Y);
            return distanceX > distanceY ? distanceY - distanceX : distanceX - distanceY;
        }

        /// <summary>
        /// Determines if two nodes are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Node node && position == node.position;
        }

        /// <summary>
        /// Gets the hash code of the node.
        /// </summary>
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }

        /// <summary>
        /// Determines if two nodes are equal.
        /// </summary>
        public static bool operator ==(Node a, Node b)
        {
            return a!.position == b!.position;
        }

        /// <summary>
        /// Determines if two nodes are not equal.
        /// </summary>
        public static bool operator !=(Node a, Node b)
        {
            return a!.position != b!.position;
        }
    }

    public class Pathfinder : MonoBehaviour
    {
        public Grid grid;
        /// <summary>
        /// Finds the shortest path to the target.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <returns>The path to get to the target.</returns>
        [CanBeNull]
        public List<Node> FindPath(Vector2 target)
        {
            var path = new Dictionary<Node, Node>();
            var openSet = new List<Node>();
            var closedSet = new List<Node>();

            // Add the starting node to the open set.
            var startNode = new Node(transform.position, true);
            openSet.Add(startNode);

            // Determine the destination node.
            var destNode = new Node(target, true);

            // Determine the path.
            var startTime = Time.realtimeSinceStartup;
            while (openSet.Count != 0)
            {
                var currentNode = openSet.OrderBy(node => node.FCost).First();
                
                if (currentNode == destNode)
                {
                    return RetracePath(path, currentNode);
                }

                openSet.Remove(currentNode);
                foreach (var neighbor in FindNeighbors(currentNode))
                {
                    var gCost = currentNode.gCost + currentNode.GetDistanceTo(neighbor);
                    if (gCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = gCost;
                        neighbor.hCost = neighbor.GetDistanceTo(destNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the neighbors to a point.
        /// </summary>
        /// <param name="point">The 2D point.</param>
        /// <returns>An array of the point's neighbors.</returns>
        private List<Node> FindNeighbors(Node point)
        {
            var neighbors = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    // Skip the center node.
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    
                    var neighbor = new Vector2(point.X + x, point.Y + y);
                    neighbors.Add(new Node(neighbor, true));
                    // TODO: Check if the neighbor is walkable.
                    // TODO: Check if the neighbor exists in the cache.
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Retraces the path to the target.
        /// </summary>
        private List<Node> RetracePath(Dictionary<Node, Node> path, Node endNode)
        {
            var finalPath = new List<Node>();
            var currentNode = endNode;
            while (path.ContainsKey(currentNode))
            {
                finalPath.Add(currentNode);
                currentNode = path[currentNode];
            }

            finalPath.Reverse();
            return finalPath;
        }
    }
}