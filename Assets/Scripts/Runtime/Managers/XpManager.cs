using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Tracks shared XP and level. When XP reaches goal, shows reward UI.
    /// Skip = keep same goal; buying a reward = level up and increase goal.
    /// </summary>
    public class XpManager : MonoSingleton<XpManager>
    {
        [Header("Level curve")]
        [SerializeField] private int baseXpPerLevel = 10;
        [SerializeField] private int xpIncreasePerLevel = 5;

        private int _currentXp;
        private int _goalXp;
        private int _level = 1;
        private bool _rewardPending;

        public int CurrentXp => _currentXp;
        public int GoalXp => _goalXp;
        public int Level => _level;
        public int XpTillNext => Mathf.Max(0, _goalXp - _currentXp);
        public float NormalizedXp => _goalXp > 0 ? Mathf.Clamp01((float)_currentXp / _goalXp) : 0f;
        public bool RewardPending => _rewardPending;

        public static event System.Action OnLevelUp;
        public static event System.Action OnRewardReady;

        private void Start()
        {
            SetGoalForLevel(1);
        }

        public void AddXp(int amount)
        {
            if (amount <= 0 || _rewardPending) return;
            var mult = UpgradeManager.HasInstance() ? UpgradeManager.Instance.GetXpMultiplier() : 1f;
            var finalAmount = Mathf.RoundToInt(amount * mult);
            if (finalAmount < 1) return;
            _currentXp += finalAmount;
            if (_currentXp >= _goalXp)
            {
                _rewardPending = true;
                OnRewardReady?.Invoke();
            }
        }

        /// <summary>Skip: don't level up, reset current to 0 and keep same goal (so they need to earn goal again).</summary>
        public void SkipReward()
        {
            if (!_rewardPending) return;
            _rewardPending = false;
            _currentXp = 0;
            // Goal stays same
        }

        /// <summary>Buy a reward: level up, set new goal for next level, reset current XP.</summary>
        public void PurchaseReward()
        {
            if (!_rewardPending) return;
            _rewardPending = false;
            _currentXp = 0;
            _level++;
            SetGoalForLevel(_level);
            OnLevelUp?.Invoke();
        }

        private void SetGoalForLevel(int level)
        {
            _goalXp = baseXpPerLevel + (level - 1) * xpIncreasePerLevel;
        }
    }
}
