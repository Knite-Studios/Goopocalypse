using UnityEngine;

namespace Runtime.UI
{
    /// <summary>
    /// This simulates random idle movement for a set of enemy entities
    /// in the game's background in a main menu scene. The enemies will move in random directions
    /// without overlapping/colliding with each other, the movement will be continous and not ping ponging back and forth.
    /// </summary>
    public class EnemyIdler : MonoBehaviour
    {
        [Header("Enemy Settings")]
        [Tooltip("List of all enemy transforms to manage.")]
        [SerializeField] private Transform[] enemyTransforms;

        [Header("Movement Settings")]
        [Tooltip("Speed at which the enemy moves.")]
        [SerializeField] private float moveSpeed = 5.0f;

        [Tooltip("Time between direction changes.")]
        [SerializeField] private float directionChangeInterval = 2.0f;

        [Tooltip("The damping effect on movement, the higher, the smoother.")]
        [SerializeField] private float damping = 1.0f;

        [Tooltip("Minimum distance to keep from other mobs.")]
        [SerializeField] private float minDistanceFromOthers = 2.0f;
    }
}
