using System;
using Systems.Attributes;
using UnityEngine;
using Attribute = Systems.Attributes.Attribute;

namespace Scriptable
{
    /// <summary>
    /// A single upgrade that can be applied to a player.
    /// Each upgrade applies one or more stat modifiers via the Attribute system.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "Scriptable/Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        [Header("Display")]
        public string upgradeName;
        [TextArea(2, 4)] public string description;
        public Sprite icon;

        [Header("Cost")]
        [Min(0)] public long orbCost;

        [Header("Requirements")]
        [Tooltip("Upgrades that must be purchased before this one is available")]
        public UpgradeData[] prerequisites;

        [Header("Effects")]
        public StatModifier[] modifiers;

        [Header("Constraints")]
        [Tooltip("Max times this upgrade can be purchased (0 = unlimited)")]
        [Min(0)] public int maxStacks = 1;
    }

    /// <summary>
    /// Defines a single stat modification: which attribute, the operation, and the value.
    /// </summary>
    [Serializable]
    public struct StatModifier
    {
        public Attribute attribute;
        public Operation operation;

        [Tooltip("Int value (for MaxHealth, Armor, etc.)")]
        public int intValue;

        [Tooltip("Float value (for Speed, Stamina, AoE, etc.)")]
        public float floatValue;
    }
}
