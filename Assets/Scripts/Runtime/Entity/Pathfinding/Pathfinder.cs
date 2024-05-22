using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Priority_Queue;
using UnityEngine;

namespace Entity.Pathfinding
{
    [Serializable]
    public class Node : FastPriorityQueueNode
    {
        [CanBeNull] public Node parent;
        public readonly Vector2 gridPosition;
        public readonly Vector2 worldPosition;
        public bool isWalkable;
        public int gCost, hCost;

        public int FCost => gCost + hCost;
        public float X => worldPosition.x;
        public float Y => worldPosition.y;

        public Node(Vector2 gridPosition, Vector2 worldPosition, bool isWalkable)
        {
            this.gridPosition = gridPosition;
            this.worldPosition = worldPosition;
            this.isWalkable = isWalkable;
        }

        /// <summary>
        /// Gets the distance to another node.
        /// </summary>
        /// <param name="other">The target node.</param>
        /// <param name="distanceType">The type of distance calculation.</param>
        /// <returns>The distance to the target node.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the distance type is invalid.</exception>
        public int GetDistanceTo(Node other, DistanceType distanceType = DistanceType.Manhattan)
        {
            var distanceX = (int)Mathf.Abs(gridPosition.x - other.gridPosition.x);
            var distanceY = (int)Mathf.Abs(gridPosition.y - other.gridPosition.y);

            return distanceType switch
            {
                DistanceType.Basic => distanceX + distanceY,
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
            return obj is Node node && worldPosition == node.worldPosition;
        }

        /// <summary>
        /// Gets the hash code of the node.
        /// </summary>
        public override int GetHashCode()
        {
            return worldPosition.GetHashCode();
        }

        /// <summary>
        /// Determines if two nodes are equal.
        /// </summary>
        public static bool operator ==(Node a, Node b)
        {
            return a?.worldPosition == b?.worldPosition;
        }

        /// <summary>
        /// Determines if two nodes are not equal.
        /// </summary>
        public static bool operator !=(Node a, Node b)
        {
            return a?.worldPosition != b?.worldPosition;
        }

        public enum DistanceType
        {
            Basic,
            Manhattan,
            Euclidean,
        }
    }

    public class Pathfinder : MonoBehaviour
    {
        [CanBeNull] public Grid grid;
        public float dynamicPadding = 0.5f;

        private List<Node> _currentPath;

        private void Awake()
        {
            grid = FindObjectOfType<Grid>();
            if (grid == null)
            {
                Debug.LogError("No grid found in the scene.");
            }
        }

        /// <summary>
        /// Finds the shortest path to the target.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <returns>The path to get to the target.</returns>
        [CanBeNull]
        public List<Node> FindPath(Vector2 target)
        {
            var openSet = new FastPriorityQueue<Node>(grid!.width * grid.height);
            var closedSet = new HashSet<Node>();

            // Add the starting node to the open set.
            var startNode = grid!.GetNode(transform.position);
            // Determine the destination node.
            var destNode = grid.GetNode(target);

            if (startNode == null || destNode == null || !startNode.isWalkable || !destNode.isWalkable)
            {
                return null;
            }

            openSet.Enqueue(startNode, startNode.FCost);

            while (openSet.Count > 0)
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

                    // Dynamically check for obstacles along the path.
                    var worldPosition = new Vector3(
                        neighbor.X * grid.nodeDiameter + grid.nodeRadius,
                        neighbor.Y * grid.nodeDiameter + grid.nodeRadius,
                        0);
                    neighbor.isWalkable = !Physics2D.OverlapCircle(worldPosition, grid.nodeRadius, grid.unwalkableLayer);

                    if (!neighbor.isWalkable) continue;

                    if (neighbor != destNode && IsNearUnwalkableNode(neighbor, dynamicPadding)) continue;

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
        /// Checks if a node is near an unwalkable node within the specified padding distance.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <param name="padding">The padding distance.</param>
        /// <returns>True if the node is near an unwalkable node; false otherwise.</returns>
        private bool IsNearUnwalkableNode(Node node, float padding)
        {
            for (var i = Mathf.FloorToInt(-padding); i <= Mathf.CeilToInt(padding); i++)
            {
                for (var j = Mathf.FloorToInt(-padding); j <= Mathf.CeilToInt(padding); j++)
                {
                    if (i == 0 && j == 0) continue;

                    var checkX = Mathf.RoundToInt(node.gridPosition.x) + i;
                    var checkY = Mathf.RoundToInt(node.gridPosition.y) + j;

                    if (checkX >= 0 && checkX < grid.width && checkY >= 0 && checkY < grid.height)
                    {
                        var checkNode = grid.GetNode(checkX, checkY);
                        if (checkNode != null && !checkNode.isWalkable)
                        {
                            var worldPosition = new Vector3(
                                node.X * grid.nodeDiameter + grid.nodeRadius,
                                node.Y * grid.nodeDiameter + grid.nodeRadius,
                                0);
                            var targetPosition = new Vector3(
                                checkX * grid.nodeDiameter + grid.nodeRadius,
                                checkY * grid.nodeDiameter + grid.nodeRadius,
                                0);

                            if (Vector3.Distance(worldPosition, targetPosition) <= padding * grid.nodeDiameter)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
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
                if (currentNode != null) currentNode = currentNode.parent;
            }

            finalPath.Reverse();
            return finalPath;
        }

        private void OnDrawGizmos()
        {
            if (_currentPath == null) return;

            Gizmos.color = Color.cyan;
            foreach (var node in _currentPath)
            {
                Gizmos.DrawSphere(new Vector3(node.X, node.Y, 0), 0.3f);
            }
        }
    }
}
