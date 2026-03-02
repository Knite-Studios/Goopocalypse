using UnityEngine;

namespace Scriptable
{
    /// <summary>
    /// Stats for enemy entities. Extends base stats with score points.
    /// Assigned per-enemy-prefab in the inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Scriptable/Enemy Stats")]
    public class EnemyStatsData : EntityStatsData
    {
        [Header("Enemy")]
        [Tooltip("Score awarded to the player on kill")]
        [Min(0)] public long points;
    }
}
