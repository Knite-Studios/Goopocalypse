using Managers;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public enum ButtonAction
    {
        PlayLocal,
        PlayOnline,
        Credits,
        Settings,
        Leaderboard,
        Exit
    }

    [Header("Button Configuration")]
    public ButtonAction action;

    private Button _button;
    private AudioManager _audioManager;

    void Start()
    {
        _button = GetComponent<Button>();
        _audioManager = AudioManager.Instance;

        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        PlayClickSound();

        switch (action)
        {
            case ButtonAction.PlayLocal:
                GameManager.Instance.StartLocalGame();
                break;

            case ButtonAction.PlayOnline:
                // For now, just start local game until we implement lobby UI
                GameManager.Instance.StartLocalGame();
                // LobbyManager.Instance.MakeLobby();
                break;

            case ButtonAction.Credits:
                // Show credits screen directly
                UIManager.Instance.ShowCreditsScreen();
                break;

            case ButtonAction.Settings:
                // Show settings screen directly
                UIManager.Instance.ShowSettingsScreen();
                break;

            case ButtonAction.Leaderboard:
                Debug.Log("Leaderboard button clicked - implement ");
                break;

            case ButtonAction.Exit:
                GameManager.Instance.QuitGame();
                break;
        }
    }

    void PlayClickSound()
    {
        if (_audioManager != null)
        {
            _audioManager.PlayUIClickSound();
        }
    }
}
