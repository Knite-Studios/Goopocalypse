using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls the pause menu during gameplay.
    /// Press ESC to toggle pause state.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private bool _isPaused;

        private void Start()
        {
            if (pauseMenuPanel)
                pauseMenuPanel.SetActive(false);

            if (resumeButton)
                resumeButton.onClick.AddListener(Resume);
            if (settingsButton)
                settingsButton.onClick.AddListener(OpenSettings);
            if (quitButton)
                quitButton.onClick.AddListener(QuitToMainMenu);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        private void Pause()
        {
            _isPaused = true;

            if (pauseMenuPanel)
                pauseMenuPanel.SetActive(true);

            Time.timeScale = 0f;  // Freeze game

            if (GameManager.HasInstance())
                GameManager.Instance.State = GameState.Paused;
        }

        private void Resume()
        {
            _isPaused = false;

            if (pauseMenuPanel)
                pauseMenuPanel.SetActive(false);

            Time.timeScale = 1f;  // Resume game

            if (GameManager.HasInstance())
                GameManager.Instance.State = GameState.Playing;
        }

        private void OpenSettings()
        {
            // TODO: Show settings overlay in pause menu
            Debug.Log("Settings not implemented in pause menu yet");
        }

        private async void QuitToMainMenu()
        {
            Time.timeScale = 1f;  // Reset time scale

            if (!GameManager.HasInstance()) return;

            if (GameManager.Instance.LocalMultiplayer)
            {
                // Local co-op: just load main menu
                await GameManager.Instance.LoadScene(0);
            }
            else
            {
                // Online: disconnect from lobby first
                if (LobbyManager.HasInstance())
                    LobbyManager.Instance.LeaveLobby();

                await GameManager.Instance.LoadScene(0);
            }
        }

        private void OnDestroy()
        {
            if (resumeButton)
                resumeButton.onClick.RemoveListener(Resume);
            if (settingsButton)
                settingsButton.onClick.RemoveListener(OpenSettings);
            if (quitButton)
                quitButton.onClick.RemoveListener(QuitToMainMenu);

            // Ensure time scale is reset
            Time.timeScale = 1f;
        }
    }
}
