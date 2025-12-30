using Managers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu screen buttons.
/// Attach to MainMenuScreen GameObject in the scene.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playLocalButton;
    [SerializeField] private Button playOnlineButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        // Wire up button listeners
        if (playLocalButton != null)
            playLocalButton.onClick.AddListener(OnPlayLocalClicked);
        if (playOnlineButton != null)
            playOnlineButton.onClick.AddListener(OnPlayOnlineClicked);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsClicked);
        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(OnLeaderboardClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDisable()
    {
        // Remove button listeners
        if (playLocalButton != null)
            playLocalButton.onClick.RemoveListener(OnPlayLocalClicked);
        if (playOnlineButton != null)
            playOnlineButton.onClick.RemoveListener(OnPlayOnlineClicked);
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
        if (creditsButton != null)
            creditsButton.onClick.RemoveListener(OnCreditsClicked);
        if (leaderboardButton != null)
            leaderboardButton.onClick.RemoveListener(OnLeaderboardClicked);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnPlayLocalClicked()
    {
        Debug.Log("Play Local clicked");
        if (GameManager.HasInstance())
            GameManager.Instance.StartLocalGame();
    }

    private void OnPlayOnlineClicked()
    {
        Debug.Log("Play Online clicked");
        if (UIManager.HasInstance())
            UIManager.Instance.ShowLobbyScreen();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings clicked");
        if (UIManager.HasInstance())
            UIManager.Instance.ShowSettingsScreen();
    }

    private void OnCreditsClicked()
    {
        Debug.Log("Credits clicked");
        if (UIManager.HasInstance())
            UIManager.Instance.ShowCreditsScreen();
    }

    private void OnLeaderboardClicked()
    {
        Debug.Log("Leaderboard clicked");
        if (UIManager.HasInstance())
            UIManager.Instance.ShowLeaderboardScreen();
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit clicked");
        if (GameManager.HasInstance())
            GameManager.Instance.QuitGame();
    }
}
