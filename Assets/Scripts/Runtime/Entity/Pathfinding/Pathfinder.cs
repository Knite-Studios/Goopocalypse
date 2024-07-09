using System.Collections.Generic;
using JetBrains.Annotations;
using Priority_Queue;
using UnityEngine;

namespace Entity.Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        [CanBeNull] public PathfindingGrid grid;

        private List<Node> _currentPath;

        private void Awake()
        {
            if (grid) return;

            grid = FindObjectOfType<PathfindingGrid>();
            if (grid == null)
                Debug.LogError("No grid found in the scene.");
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_currentPath == null) return;

            Gizmos.color = Color.cyan;
            foreach (var node in _currentPath)
            {
                Gizmos.DrawSphere(new Vector3(node.X, node.Y, 0), 0.3f);
            }
        }
#endif

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
            if (startNode == null) return null;

            // Determine the destination node.
            var destNode = grid.GetNode(target);
            if (destNode == null || !startNode.isWalkable || !destNode.isWalkable)
                return null;

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
                    // var worldPosition = new Vector3(
                    //     neighbor.X * grid.nodeDiameter + grid.nodeRadius,
                    //     neighbor.Y * grid.nodeDiameter + grid.nodeRadius,
                    //     0);

                    // neighbor.isWalkable = !Physics2D.OverlapCircle(worldPosition, grid.nodeRadius, grid.unwalkableLayer);
                    // if (!neighbor.isWalkable) continue;

                    var gCost = currentNode.gCost + currentNode.GetDistanceTo(neighbor);
                    if (gCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = gCost;
                        neighbor.hCost = neighbor.GetDistanceTo(destNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                            openSet.Enqueue(neighbor, neighbor.FCost);
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
                if (currentNode != null) currentNode = currentNode.parent;
            }

            finalPath.Reverse();
            return finalPath;
        }
    }
}
