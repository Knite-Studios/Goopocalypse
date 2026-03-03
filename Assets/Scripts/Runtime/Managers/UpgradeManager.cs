using System.Collections.Generic;
using Scriptable;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Holds the upgrade pool, tracks purchased upgrades, and exposes effect getters for Link, speed, courage, and XP.
    /// </summary>
    public class UpgradeManager : MonoSingleton<UpgradeManager>
    {
        [Header("Upgrade pool")]
        [Tooltip("All available upgrades. If empty, loads from Resources/Upgrades.")]
        [SerializeField] private List<UpgradeDefinition> upgradePool = new List<UpgradeDefinition>();

        private readonly List<UpgradeDefinition> _appliedUpgrades = new List<UpgradeDefinition>();

        /// <summary> All upgrades that have been purchased this run. </summary>
        public IReadOnlyList<UpgradeDefinition> AppliedUpgrades => _appliedUpgrades;

        private void Awake()
        {
            if (upgradePool.Count == 0)
                LoadUpgradesFromResources();
        }

        private void LoadUpgradesFromResources()
        {
            var loaded = Resources.LoadAll<UpgradeDefinition>("Upgrades");
            if (loaded != null && loaded.Length > 0)
                upgradePool.AddRange(loaded);
        }

        /// <summary>
        /// Returns N random upgrade definitions from the pool. Does not exclude already purchased (stacks allowed).
        /// </summary>
        public List<UpgradeDefinition> GetRandomUpgrades(int count)
        {
            var result = new List<UpgradeDefinition>();
            if (upgradePool.Count == 0) return result;

            for (var i = 0; i < count; i++)
            {
                var index = Random.Range(0, upgradePool.Count);
                result.Add(upgradePool[index]);
            }
            return result;
        }

        /// <summary>
        /// True if current orbs are enough to purchase the upgrade.
        /// </summary>
        public bool CanAfford(UpgradeDefinition definition, long currentOrbs)
        {
            return definition != null && currentOrbs >= definition.costInOrbs;
        }

        /// <summary>
        /// Deducts orbs via WaveManager, records the upgrade, and applies effect (via getters read by Link, movement, etc.).
        /// Returns true if applied, false if cannot afford or missing managers.
        /// </summary>
        public bool Apply(UpgradeDefinition definition)
        {
            if (definition == null) return false;
            if (!WaveManager.HasInstance()) return false;
            var currentOrbs = WaveManager.Instance.Score;
            if (!CanAfford(definition, currentOrbs)) return false;

            WaveManager.Instance.DeductScore(definition.costInOrbs);
            _appliedUpgrades.Add(definition);
            return true;
        }

        #region Effect getters (used by Link, PlayerBaseState, UltimateManager, XpManager)

        /// <summary> Additive bonus to link max distance (meters). Base is on Link; add this sum. </summary>
        public float GetLinkLengthBonus()
        {
            var sum = 0f;
            foreach (var u in _appliedUpgrades)
            {
                if (u.upgradeType == UpgradeType.LinkLength)
                    sum += u.value;
            }
            return sum;
        }

        /// <summary> Multiplier for player movement speed (1 = no change). Apply as baseSpeed * this. </summary>
        public float GetMoveSpeedMultiplier()
        {
            var sum = 0f;
            foreach (var u in _appliedUpgrades)
            {
                if (u.upgradeType == UpgradeType.MoveSpeed)
                    sum += u.value;
            }
            return 1f + sum;
        }

        /// <summary> Multiplier for courage/ultimate charge rate (1 = no change). </summary>
        public float GetCourageGainMultiplier()
        {
            var sum = 0f;
            foreach (var u in _appliedUpgrades)
            {
                if (u.upgradeType == UpgradeType.CourageGain)
                    sum += u.value;
            }
            return 1f + sum;
        }

        /// <summary> Multiplier for XP from orbs (1 = no change). </summary>
        public float GetXpMultiplier()
        {
            var sum = 0f;
            foreach (var u in _appliedUpgrades)
            {
                if (u.upgradeType == UpgradeType.XpFromOrbs)
                    sum += u.value;
            }
            return 1f + sum;
        }

        #endregion
    }
}
