using System;
using System.Collections.Generic;
using Common.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Utils;

namespace Entity
{
    [Serializable]
    public class Node
    {
        [CanBeNull] public Node parent;
        public readonly Vector2Int position;
        public bool isWalkable;
        public int gCost, hCost;
        
        public int FCost => gCost + hCost;

        public int X => position.x;
        public int Y => position.y;
        
        public Node(Vector2Int position, bool isWalkable)
        {
            this.position = position;
            this.isWalkable = isWalkable;
        }

        /// <summary>
        /// </summary>
        public int GetDistanceTo(Node other, DistanceType distanceType = DistanceType.Manhattan)
        {
            var distanceX = Mathf.Abs(X - other.X);
            var distanceY = Mathf.Abs(Y - other.Y);
            
            return distanceType switch
            {
                DistanceType.Manhattan => distanceX > distanceY
                    ? 14 * distanceY + 10 * (distanceX - distanceY)
                    : 14 * distanceX + 10 * (distanceY - distanceX),
                DistanceType.Euclidean => (int)Math.Sqrt(distanceX * distanceX + distanceY * distanceY),
                _ => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
            };
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
            return a?.position == b?.position;
        }

        /// <summary>
        /// Determines if two nodes are not equal.
        /// </summary>
        public static bool operator !=(Node a, Node b)
        {
            return a?.position != b?.position;
        }

        public enum DistanceType
        {
            Manhattan,
            Euclidean,
        }
    }

    public class Pathfinder : MonoBehaviour
    {
        public Grid grid;
        
        private List<Node> _currentPath;
        
        /// <summary>
        /// Finds the shortest path to the target.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <returns>The path to get to the target.</returns>
        [CanBeNull]
        public List<Node> FindPath(Vector2 target)
        {
            var openSet = new PriorityQueue<Node, int>();
            var closedSet = new HashSet<Node>();

            // Add the starting node to the open set.
            var startNode = grid.GetNode((int)transform.position.x, (int)transform.position.y);
            // Determine the destination node.
            var destNode = grid.GetNode((int)target.x, (int)target.y);
            
            if (startNode == null || destNode == null || !startNode.isWalkable || !destNode.isWalkable)
            {
                return null;
            }
            
            openSet.Enqueue(startNode, startNode.FCost);
            
            while (openSet.Count != 0)
            {
                // Get the node with the lowest F cost.
                var currentNode = openSet.Dequeue();
                
                if (currentNode == destNode)
                {
                    _currentPath = RetracePath(startNode, destNode);
                    return _currentPath;
                }
                
                closedSet.Add(currentNode);
                
                foreach (var neighbor in grid.FindNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;
                    
                    var gCost = currentNode.gCost + currentNode.GetDistanceTo(neighbor);
                    if (gCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = gCost;
                        neighbor.hCost = neighbor.GetDistanceTo(destNode);
                        neighbor.parent = currentNode;
                        
                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, neighbor.FCost);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retraces the path from the start node to the end node.
        /// </summary>
        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            var finalPath = new List<Node>();
            var currentNode = endNode;
            while (currentNode != startNode)
            {
                finalPath.Add(currentNode);
                currentNode = currentNode!.parent;
            }

            finalPath.Reverse();
            return finalPath;
        }
        
        private void OnDrawGizmos()
        {
            if (grid == null) return;
            
            Gizmos.color = Color.green;
            foreach (var node in _currentPath)
            {
                Gizmos.DrawSphere(new Vector3(node.X, node.Y, 0), 0.3f);
            }
        }
    }
}