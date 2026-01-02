using UnityEngine;

/// <summary>
/// Lightweight UIManager that delegates to MenuController.
/// Kept for backwards compatibility with existing code.
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    private MenuController _menuController;

    protected override void OnAwake()
    {
        _menuController = FindObjectOfType<MenuController>();
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ShowMainMenu()
    {
        if (_menuController != null)
            _menuController.ReturnMenu();
    }

    /// <summary>
    /// Shows the lobby screen for local coop.
    /// </summary>
    public void ShowLobbyLocal()
    {
        if (_menuController != null)
            _menuController.PlayLocalCoop();
    }

    /// <summary>
    /// Shows the lobby screen for online coop.
    /// </summary>
    public void ShowLobbyOnline()
    {
        if (_menuController != null)
            _menuController.PlayOnlineCoop();
    }

    /// <summary>
    /// Shows the extras/leaderboard screen.
    /// </summary>
    public void ShowExtras()
    {
        if (_menuController != null)
            _menuController.ExtrasMenu();
    }
}
