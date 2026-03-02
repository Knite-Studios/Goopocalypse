using UnityEngine;

namespace Scriptable
{
    /// <summary>
    /// Base stats shared by all entities (players and enemies).
    /// These values feed into the Attribute system as base values.
    /// Modifiers can be applied at runtime for upgrades/buffs.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEntityStats", menuName = "Scriptable/Entity Stats")]
    public class EntityStatsData : ScriptableObject
    {
        [Header("Core Stats")]
        [Tooltip("Maximum hit points")]
        [Min(1)] public int maxHealth = 20;

        [Tooltip("Movement speed (units/sec)")]
        [Min(0f)] public float speed = 1f;

        [Tooltip("Damage reduction from non-true-damage hits")]
        [Min(0)] public int armor;
    }
}
