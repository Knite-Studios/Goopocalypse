using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI Screens")]
    public GameObject mainMenuScreen;
    public GameObject lobbyScreen;
    public GameObject creditsScreen;
    public GameObject settingsScreen;
    public GameObject leaderboardScreen;

    protected override void OnAwake()
    {
        // Start with main menu visible
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        HideAllScreens();
        mainMenuScreen.SetActive(true);
    }

    public void ShowLobbyScreen()
    {
        HideAllScreens();
        lobbyScreen.SetActive(true);
    }

    public void ShowCreditsScreen()
    {
        HideAllScreens();
        creditsScreen.SetActive(true);
    }

    public void ShowSettingsScreen()
    {
        HideAllScreens();
        settingsScreen.SetActive(true);
    }

    public void ShowLeaderboardScreen()
    {
        HideAllScreens();
        leaderboardScreen.SetActive(true);
    }

    private void HideAllScreens()
    {
        mainMenuScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        creditsScreen.SetActive(false);
        settingsScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }

    // Optional: Back button functionality
    public void GoBackToMainMenu()
    {
        ShowMainMenu();
    }
}
