using DG.Tweening;
using UnityEngine;

/// <summary>
/// Manages UI screen visibility with smooth transitions.
/// Attach to a GameObject in the menu scene and assign screen references.
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI Screens")]
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject leaderboardScreen;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.25f;
    [SerializeField] private Ease fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease fadeOutEase = Ease.InQuad;

    private GameObject _currentScreen;
    private Tween _currentTween;

    protected override void OnAwake()
    {
        // Ensure all screens have CanvasGroup for fading
        EnsureCanvasGroup(mainMenuScreen);
        EnsureCanvasGroup(lobbyScreen);
        EnsureCanvasGroup(creditsScreen);
        EnsureCanvasGroup(settingsScreen);
        EnsureCanvasGroup(leaderboardScreen);

        // Start with main menu visible
        ShowMainMenu();
    }

    private void EnsureCanvasGroup(GameObject screen)
    {
        if (screen == null) return;
        if (screen.GetComponent<CanvasGroup>() == null)
            screen.AddComponent<CanvasGroup>();
    }

    public void ShowMainMenu() => TransitionTo(mainMenuScreen);
    public void ShowLobbyScreen() => TransitionTo(lobbyScreen);
    public void ShowCreditsScreen() => TransitionTo(creditsScreen);
    public void ShowSettingsScreen() => TransitionTo(settingsScreen);
    public void ShowLeaderboardScreen() => TransitionTo(leaderboardScreen);

    private void TransitionTo(GameObject target)
    {
        if (target == null) return;
        if (_currentScreen == target) return;

        _currentTween?.Kill();

        // Fade out current screen
        if (_currentScreen != null)
        {
            var outScreen = _currentScreen;
            var outGroup = outScreen.GetComponent<CanvasGroup>();

            if (outGroup != null)
            {
                outGroup.interactable = false;
                outGroup.blocksRaycasts = false;
                outGroup.DOFade(0f, transitionDuration * 0.5f)
                    .SetEase(fadeOutEase)
                    .SetUpdate(true)
                    .OnComplete(() => outScreen.SetActive(false));
            }
            else
            {
                outScreen.SetActive(false);
            }
        }

        // Fade in target screen
        var inGroup = target.GetComponent<CanvasGroup>();
        if (inGroup != null)
        {
            inGroup.alpha = 0f;
            inGroup.interactable = false;
            inGroup.blocksRaycasts = false;
        }

        target.SetActive(true);

        if (inGroup != null)
        {
            _currentTween = inGroup.DOFade(1f, transitionDuration)
                .SetEase(fadeInEase)
                .SetUpdate(true)
                .SetDelay(transitionDuration * 0.3f)
                .OnComplete(() =>
                {
                    inGroup.interactable = true;
                    inGroup.blocksRaycasts = true;
                });
        }

        _currentScreen = target;
    }

    /// <summary>
    /// Instantly show a screen without transition (useful for initialization).
    /// </summary>
    public void ShowInstant(GameObject screen)
    {
        if (screen == null) return;

        HideAllScreensInstant();

        var group = screen.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        screen.SetActive(true);
        _currentScreen = screen;
    }

    private void HideAllScreensInstant()
    {
        HideScreenInstant(mainMenuScreen);
        HideScreenInstant(lobbyScreen);
        HideScreenInstant(creditsScreen);
        HideScreenInstant(settingsScreen);
        HideScreenInstant(leaderboardScreen);
    }

    private void HideScreenInstant(GameObject screen)
    {
        if (screen == null) return;

        var group = screen.GetComponent<CanvasGroup>();
        if (group != null)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        screen.SetActive(false);
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
    }
}
