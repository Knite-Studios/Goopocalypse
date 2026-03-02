using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controls the in-game HUD display.
    /// Shows orbs collected and match timer.
    /// </summary>
    public class GameHUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI orbsText;
        [SerializeField] private TextMeshProUGUI timerText;

        private void OnEnable()
        {
            if (WaveManager.HasInstance())
            {
                WaveManager.OnScoreChanged += UpdateOrbsDisplay;
                WaveManager.OnMatchTimerChanged += UpdateTimerDisplay;
            }

            UpdateOrbsDisplay(WaveManager.HasInstance() ? WaveManager.Instance.Score : 0);
            UpdateTimerDisplay(WaveManager.HasInstance() ? WaveManager.Instance.MatchTimer : 0);
        }

        private void OnDisable()
        {
            if (WaveManager.HasInstance())
            {
                WaveManager.OnScoreChanged -= UpdateOrbsDisplay;
                WaveManager.OnMatchTimerChanged -= UpdateTimerDisplay;
            }
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
    }
}
