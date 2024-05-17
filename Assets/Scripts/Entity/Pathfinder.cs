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
        /// Gets the manhattan distance to another node.
        /// </summary>
        public int GetDistanceTo(Node other)
        {
            var distanceX = Mathf.Abs(X - other.X);
            var distanceY = Mathf.Abs(Y - other.Y);
            return distanceX + distanceY;
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
                    return RetracePath(startNode, destNode);
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
    }
}