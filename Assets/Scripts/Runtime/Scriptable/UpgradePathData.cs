using System;
using Entity.Player;
using UnityEngine;

namespace Scriptable
{
    /// <summary>
    /// Defines a complete upgrade tree for a specific player role.
    /// Upgrade tiers unlock sequentially as the player progresses.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgradePath", menuName = "Scriptable/Upgrade Path")]
    public class UpgradePathData : ScriptableObject
    {
        public PlayerRole role;

        [Header("Upgrade Tiers")]
        [Tooltip("Each tier is a group of upgrades available at that progression level")]
        public UpgradeTier[] tiers;
    }

    [Serializable]
    public struct UpgradeTier
    {
        public string tierName;

        [Tooltip("Wave number required to unlock this tier")]
        [Min(0)] public int waveRequirement;

        [Tooltip("Upgrades available in this tier")]
        public UpgradeData[] upgrades;
    }
}
