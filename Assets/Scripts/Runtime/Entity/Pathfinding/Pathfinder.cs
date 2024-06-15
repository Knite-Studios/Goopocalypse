using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Priority_Queue;
using UnityEngine;

namespace Entity.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        [CanBeNull] public PathfindingGrid grid;
        public float dynamicPadding = 0.5f;

        private List<Node> _currentPath;

        private void Awake()
        {
            grid = FindObjectOfType<PathfindingGrid>();
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

                    // if (!neighbor.isWalkable) continue;
                    // if (neighbor != destNode && IsNearUnwalkableNode(neighbor, dynamicPadding)) continue;

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

                    var checkX = Mathf.RoundToInt(node.GridPosition.x) + i;
                    var checkY = Mathf.RoundToInt(node.GridPosition.y) + j;

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
