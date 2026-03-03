using UnityEngine;

namespace Scriptable
{
    /// <summary>
    /// Configuration for the shared ultimate bar and boon cadence.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUltimateConfig", menuName = "Scriptable/Ultimate Config")]
    public class UltimateConfig : ScriptableObject
    {
        [Header("Charge")]
        [Min(1f)] public float ultMax = 100f;
        [Tooltip("Base charge gained per second while the link is active.")]
        [Min(0f)] public float baseChargePerSecondWhileLinked = 5f;
        [Tooltip("Additional charge gained per enemy killed while linked.")]
        [Min(0f)] public float chargePerEnemyKilledWhileLinked = 1f;

        [Header("Scaling")]
        [Tooltip("Multiplier applied to required charge after each boon.")]
        [Min(1f)] public float chargeScalingPerBoon = 1.25f;
    }
}

