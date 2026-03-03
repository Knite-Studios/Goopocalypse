using UnityEngine;

namespace Scriptable
{
    /// <summary>
    /// Twin Lights upgrade type: determines which game system is modified and how value is applied.
    /// </summary>
    public enum UpgradeType
    {
        /// <summary> Additive: Link.baseMaxDistance + sum of values. </summary>
        LinkLength,
        /// <summary> Additive or multiplier: movement speed bonus (e.g. +0.5 units or 1.1 = 10% faster). </summary>
        MoveSpeed,
        /// <summary> Multiplier: courage/ultimate charge rate (e.g. 0.1 = +10%). </summary>
        CourageGain,
        /// <summary> Multiplier: XP from orbs (e.g. 0.2 = +20%). </summary>
        XpFromOrbs
    }

    /// <summary>
    /// Scriptable definition for a Twin Lights upgrade. Used by UpgradeManager for reward panel and application.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgradeDefinition", menuName = "Scriptable/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique id for this upgrade (e.g. link_length_1).")]
        public string id;

        [Header("Display")]
        public string displayName;
        [TextArea(2, 4)] public string description;
        public Sprite icon;

        [Header("Cost")]
        [Min(0)] public long costInOrbs = 25;

        [Header("Effect")]
        public UpgradeType upgradeType;
        [Tooltip("LinkLength: additive meters. MoveSpeed: additive units or multiplier (e.g. 0.1 = 10% faster). CourageGain/XpFromOrbs: multiplier fraction (e.g. 0.1 = +10%).")]
        public float value = 1f;
    }
}
