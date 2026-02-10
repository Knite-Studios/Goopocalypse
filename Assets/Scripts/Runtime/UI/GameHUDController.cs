using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controls the in-game HUD display.
    /// Shows orbs collected, match timer, and hearts remaining.
    /// </summary>
    public class GameHUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orbsText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI heartsText;

        private void OnEnable()
        {
            // Subscribe to WaveManager events
            if (WaveManager.HasInstance())
            {
                WaveManager.OnScoreChanged += UpdateOrbsDisplay;
                WaveManager.OnMatchTimerChanged += UpdateTimerDisplay;
            }

            // Subscribe to HeartManager events (static event, always available)
            HeartManager.OnHeartsChanged += UpdateHeartsDisplay;

            // Initialize displays with current values
            UpdateOrbsDisplay(WaveManager.HasInstance() ? WaveManager.Instance.Score : 0);
            UpdateTimerDisplay(WaveManager.HasInstance() ? WaveManager.Instance.MatchTimer : 0);

            // Try to get initial hearts value, default to 3 if manager not ready
            try
            {
                UpdateHeartsDisplay(HeartManager.Instance.Hearts);
            }
            catch
            {
                UpdateHeartsDisplay(3);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from WaveManager events
            if (WaveManager.HasInstance())
            {
                WaveManager.OnScoreChanged -= UpdateOrbsDisplay;
                WaveManager.OnMatchTimerChanged -= UpdateTimerDisplay;
            }

            // Unsubscribe from HeartManager events
            HeartManager.OnHeartsChanged -= UpdateHeartsDisplay;
        }

        private void UpdateOrbsDisplay(long score)
        {
            if (orbsText)
                orbsText.text = $"Orbs: {score}";
        }

        private void UpdateTimerDisplay(long seconds)
        {
            if (timerText)
            {
                var minutes = seconds / 60;
                var secs = seconds % 60;
                timerText.text = $"{minutes:00}:{secs:00}";
            }
        }

        private void UpdateHeartsDisplay(int hearts)
        {
            if (heartsText)
                heartsText.text = $"❤ {hearts}";  // Heart symbol + count
        }
    }
}
