using System;
using JetBrains.Annotations;
using Priority_Queue;
using UnityEngine;

namespace Entity.Pathfinding
{
    [Serializable]
    public class Node : FastPriorityQueueNode
    {
        [CanBeNull] public Node parent;
        public readonly Vector2 GridPosition;
        public readonly Vector2 WorldPosition;
        public bool isWalkable;
        public int gCost, hCost;

        public int FCost => gCost + hCost;
        public float X => WorldPosition.x;
        public float Y => WorldPosition.y;

        public Node(Vector2 gridPosition, Vector2 worldPosition, bool isWalkable)
        {
            this.GridPosition = gridPosition;
            this.WorldPosition = worldPosition;
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
            var distanceX = (int)Mathf.Abs(GridPosition.x - other.GridPosition.x);
            var distanceY = (int)Mathf.Abs(GridPosition.y - other.GridPosition.y);

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
            return obj is Node node && WorldPosition == node.WorldPosition;
        }

        /// <summary>
        /// Gets the hash code of the node.
        /// </summary>
        public override int GetHashCode()
        {
            return WorldPosition.GetHashCode();
        }

        /// <summary>
        /// Determines if two nodes are equal.
        /// </summary>
        public static bool operator ==(Node a, Node b)
        {
            return a?.WorldPosition == b?.WorldPosition;
        }

        /// <summary>
        /// Determines if two nodes are not equal.
        /// </summary>
        public static bool operator !=(Node a, Node b)
        {
            return a?.WorldPosition != b?.WorldPosition;
        }

        public enum DistanceType
        {
            Basic,
            Manhattan,
            Euclidean,
        }
    }
}
