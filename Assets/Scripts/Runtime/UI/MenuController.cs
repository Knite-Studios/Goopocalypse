using System.Collections;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main menu controller for Goopocalypse.
/// Handles menu navigation, settings panels, and lobby transitions.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Main Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject firstMenu;
    [SerializeField] private GameObject exitMenu;
    [SerializeField] private GameObject extrasMenu;
    [SerializeField] private GameObject lobbyMenu;

    [Header("Settings Canvas")]
    [SerializeField] private GameObject optionsCanvas;

    [Header("Settings Panels")]
    [SerializeField] private GameObject panelGame;
    [SerializeField] private GameObject panelVideo;
    [SerializeField] private GameObject panelControls;
    [SerializeField] private GameObject panelKeyBindings;
    [SerializeField] private GameObject panelMovement;
    [SerializeField] private GameObject panelCombat;
    [SerializeField] private GameObject panelGeneral;

    [Header("Settings Tab Highlights")]
    [SerializeField] private GameObject lineGame;
    [SerializeField] private GameObject lineVideo;
    [SerializeField] private GameObject lineControls;
    [SerializeField] private GameObject lineKeyBindings;
    [SerializeField] private GameObject lineMovement;
    [SerializeField] private GameObject lineCombat;
    [SerializeField] private GameObject lineGeneral;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingMenu;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_Text loadPromptText;
    [SerializeField] private KeyCode userPromptKey = KeyCode.Space;
    [SerializeField] private bool waitForInput = true;

    [Header("Audio")]
    [SerializeField] private AudioSource hoverSound;
    [SerializeField] private AudioSource sliderSound;
    [SerializeField] private AudioSource swooshSound;

    private Animator _cameraAnimator;
    private bool _isOnlineMode;

    public bool IsOnlineMode => _isOnlineMode;

    private void Start()
    {
        _cameraAnimator = GetComponent<Animator>();

        // Initialize menu state
        if (exitMenu) exitMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        if (lobbyMenu) lobbyMenu.SetActive(false);
        if (firstMenu) firstMenu.SetActive(true);
        if (mainMenu) mainMenu.SetActive(true);
    }

    #region Main Menu Navigation

    public void ReturnMenu()
    {
        if (extrasMenu) extrasMenu.SetActive(false);
        if (exitMenu) exitMenu.SetActive(false);
        if (lobbyMenu) lobbyMenu.SetActive(false);
        if (mainMenu) mainMenu.SetActive(true);
        if (firstMenu) firstMenu.SetActive(true);
    }

    public void ExtrasMenu()
    {
        if (extrasMenu) extrasMenu.SetActive(true);
        if (exitMenu) exitMenu.SetActive(false);
    }

    /// <summary>
    /// Loads the Leaderboard scene. Call from Extras menu or main menu Leaderboard button.
    /// </summary>
    public void OpenLeaderboard()
    {
        LoadScene("02_Leaderboard");
    }

    public void ShowExitConfirm()
    {
        if (exitMenu) exitMenu.SetActive(true);
        if (extrasMenu) extrasMenu.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Lobby / Coop

    public void PlayLocalCoop()
    {
        _isOnlineMode = false;
        if (exitMenu) exitMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        if (firstMenu) firstMenu.SetActive(false);
        if (lobbyMenu) lobbyMenu.SetActive(true);
    }

    public void PlayOnlineCoop()
    {
        _isOnlineMode = true;
        if (exitMenu) exitMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        if (firstMenu) firstMenu.SetActive(false);
        if (lobbyMenu) lobbyMenu.SetActive(true);

        // Create Steam lobby
        if (LobbyManager.HasInstance())
            LobbyManager.Instance.MakeLobby();
    }

    public void ReturnFromLobby()
    {
        if (lobbyMenu) lobbyMenu.SetActive(false);
        if (firstMenu) firstMenu.SetActive(true);

        // Leave Steam lobby if online
        if (_isOnlineMode && LobbyManager.HasInstance())
            LobbyManager.Instance.LeaveLobby();
    }

    public void InvitePlayer()
    {
        if (_isOnlineMode && LobbyManager.HasInstance())
            LobbyManager.Instance.InvitePlayer();
    }

    public void StartGame()
    {
        if (_isOnlineMode)
        {
            if (GameManager.HasInstance())
                GameManager.Instance.StartRemoteGame();
        }
        else
        {
            if (GameManager.HasInstance())
                GameManager.Instance.StartLocalGame();
        }
    }

    #endregion

    #region Settings Panels

    /// <summary>
    /// Opens the settings menu with Game panel as default.
    /// Call this from the Settings button.
    /// </summary>
    public void OpenSettings()
    {
        // Hide main menu elements
        if (firstMenu) firstMenu.SetActive(false);
        if (exitMenu) exitMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        if (lobbyMenu) lobbyMenu.SetActive(false);

        // Show options canvas with default panel
        if (optionsCanvas) optionsCanvas.SetActive(true);
        GamePanel();
    }

    /// <summary>
    /// Closes the settings menu and returns to main menu.
    /// </summary>
    public void CloseSettings()
    {
        // Reset to default panel for next time
        DisableAllPanels();
        if (panelGame) panelGame.SetActive(true);
        if (lineGame) lineGame.SetActive(true);

        // Hide options canvas
        if (optionsCanvas) optionsCanvas.SetActive(false);

        // Show main menu
        if (firstMenu) firstMenu.SetActive(true);
    }

    public void GamePanel()
    {
        DisableAllPanels();
        if (panelGame) panelGame.SetActive(true);
        if (lineGame) lineGame.SetActive(true);
    }

    public void VideoPanel()
    {
        DisableAllPanels();
        if (panelVideo) panelVideo.SetActive(true);
        if (lineVideo) lineVideo.SetActive(true);
    }

    public void ControlsPanel()
    {
        DisableAllPanels();
        if (panelControls) panelControls.SetActive(true);
        if (lineControls) lineControls.SetActive(true);
    }

    public void KeyBindingsPanel()
    {
        DisableAllPanels();
        MovementPanel();
        if (panelKeyBindings) panelKeyBindings.SetActive(true);
        if (lineKeyBindings) lineKeyBindings.SetActive(true);
    }

    public void MovementPanel()
    {
        DisableAllPanels();
        if (panelKeyBindings) panelKeyBindings.SetActive(true);
        if (panelMovement) panelMovement.SetActive(true);
        if (lineMovement) lineMovement.SetActive(true);
    }

    public void CombatPanel()
    {
        DisableAllPanels();
        if (panelKeyBindings) panelKeyBindings.SetActive(true);
        if (panelCombat) panelCombat.SetActive(true);
        if (lineCombat) lineCombat.SetActive(true);
    }

    public void GeneralPanel()
    {
        DisableAllPanels();
        if (panelKeyBindings) panelKeyBindings.SetActive(true);
        if (panelGeneral) panelGeneral.SetActive(true);
        if (lineGeneral) lineGeneral.SetActive(true);
    }

    private void DisableAllPanels()
    {
        if (panelGame) panelGame.SetActive(false);
        if (panelVideo) panelVideo.SetActive(false);
        if (panelControls) panelControls.SetActive(false);
        if (panelKeyBindings) panelKeyBindings.SetActive(false);
        if (panelMovement) panelMovement.SetActive(false);
        if (panelCombat) panelCombat.SetActive(false);
        if (panelGeneral) panelGeneral.SetActive(false);

        if (lineGame) lineGame.SetActive(false);
        if (lineVideo) lineVideo.SetActive(false);
        if (lineControls) lineControls.SetActive(false);
        if (lineKeyBindings) lineKeyBindings.SetActive(false);
        if (lineMovement) lineMovement.SetActive(false);
        if (lineCombat) lineCombat.SetActive(false);
        if (lineGeneral) lineGeneral.SetActive(false);
    }

    #endregion

    #region Camera Animation

    public void CameraPosition1()
    {
        if (_cameraAnimator) _cameraAnimator.SetFloat("Animate", 0);
    }

    public void CameraPosition2()
    {
        if (_cameraAnimator) _cameraAnimator.SetFloat("Animate", 1);
    }

    #endregion

    #region Audio

    public void PlayHover()
    {
        if (hoverSound) hoverSound.Play();
    }

    public void PlaySliderSound()
    {
        if (sliderSound) sliderSound.Play();
    }

    public void PlaySwoosh()
    {
        if (swooshSound) swooshSound.Play();
    }

    #endregion

    #region Scene Loading

    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
            StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // Hide all menus during loading
        if (firstMenu) firstMenu.SetActive(false);
        if (optionsCanvas) optionsCanvas.SetActive(false);
        if (lobbyMenu) lobbyMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        if (exitMenu) exitMenu.SetActive(false);
        if (loadingMenu) loadingMenu.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar) loadingBar.value = progress;

            if (operation.progress >= 0.9f)
            {
                if (loadingBar) loadingBar.value = 1f;

                if (waitForInput)
                {
                    if (loadPromptText)
                        loadPromptText.text = $"Press {userPromptKey.ToString().ToUpper()} to continue";

                    if (Input.GetKeyDown(userPromptKey))
                        operation.allowSceneActivation = true;
                }
                else
                {
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    #endregion
}
