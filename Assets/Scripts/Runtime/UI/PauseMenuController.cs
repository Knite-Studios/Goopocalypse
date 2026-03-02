using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls the pause menu UI during gameplay.
    /// Reacts to GameManager.OnGamePause. All UI must be built in the scene; assign references in the Inspector.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OpenSettings);
            if (quitButton != null)
                quitButton.onClick.AddListener(QuitToMainMenu);
        }

        private void OnEnable()
        {
            GameManager.OnGamePause += HandlePauseChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGamePause -= HandlePauseChanged;
        }

        private void HandlePauseChanged(bool paused)
        {
            if (GameManager.HasInstance() && GameManager.Instance.State == GameState.GameOver)
                return;
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(paused);
        }

        private void OnResumeClicked()
        {
            if (GameManager.HasInstance())
                GameManager.Instance.ResumeGame();
        }

        private void OpenSettings()
        {
            // Optional: open same settings as main menu (e.g. show options canvas or a dedicated in-game settings panel).
            if (settingsButton != null)
                Debug.Log("Pause Settings: assign a panel or open options in the scene.");
        }

        private void QuitToMainMenu()
        {
            if (GameManager.HasInstance())
                GameManager.Instance.StopGame();
        }

        private void OnDestroy()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveListener(OnResumeClicked);
            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OpenSettings);
            if (quitButton != null)
                quitButton.onClick.RemoveListener(QuitToMainMenu);
        }
    }
}
