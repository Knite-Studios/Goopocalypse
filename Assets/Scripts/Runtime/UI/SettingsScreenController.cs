using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the settings screen.
/// Attach to SettingsScreen GameObject in the scene.
/// </summary>
public class SettingsScreenController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Display")]
    [SerializeField] private TMP_Dropdown displayModeDropdown;

    [Header("Keybinds")]
    [SerializeField] private Transform keybindContainer;
    [SerializeField] private KeybindEntryUI keybindEntryPrefab;
    [SerializeField] private Button resetAllKeybindsButton;

    [Header("Navigation")]
    [SerializeField] private Button backButton;

    private readonly List<KeybindEntryUI> _keybindEntries = new List<KeybindEntryUI>();

    private void OnEnable()
    {
        InitializeUI();
        BindEvents();
        PopulateKeybinds();
    }

    private void OnDisable()
    {
        UnbindEvents();
    }

    private void Update()
    {
        // Escape to go back (only if not rebinding)
        if (Input.GetKeyDown(KeyCode.Escape) && !InputManager.IsRebinding)
        {
            OnBackClicked();
        }
    }

    private void InitializeUI()
    {
        if (!SettingsManager.HasInstance()) return;

        var settings = SettingsManager.Instance;

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(settings.MusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(settings.SoundFxVolume);
        }

        if (displayModeDropdown != null)
        {
            displayModeDropdown.SetValueWithoutNotify((int)settings.Display);
        }
    }

    private void BindEvents()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        if (displayModeDropdown != null)
            displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        if (resetAllKeybindsButton != null)
            resetAllKeybindsButton.onClick.AddListener(OnResetAllKeybinds);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    private void UnbindEvents()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
        if (displayModeDropdown != null)
            displayModeDropdown.onValueChanged.RemoveListener(OnDisplayModeChanged);
        if (resetAllKeybindsButton != null)
            resetAllKeybindsButton.onClick.RemoveListener(OnResetAllKeybinds);
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void PopulateKeybinds()
    {
        if (keybindContainer == null || keybindEntryPrefab == null) return;
        if (!InputManager.HasInstance()) return;

        // Clear existing entries
        foreach (var entry in _keybindEntries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }
        _keybindEntries.Clear();

        // Create entries for each rebindable action
        foreach (var action in InputManager.RebindableActions)
        {
            if (action == null) continue;

            var entry = Instantiate(keybindEntryPrefab, keybindContainer);
            entry.Initialize(action, "Keyboard");
            _keybindEntries.Add(entry);
        }
    }

    private void OnMusicChanged(float value)
    {
        if (SettingsManager.HasInstance())
            SettingsManager.Instance.MusicVolume = value;
    }

    private void OnSFXChanged(float value)
    {
        if (SettingsManager.HasInstance())
            SettingsManager.Instance.SoundFxVolume = value;
    }

    private void OnDisplayModeChanged(int index)
    {
        if (SettingsManager.HasInstance())
            SettingsManager.Instance.Display = (SettingsManager.DisplayMode)index;
    }

    private void OnResetAllKeybinds()
    {
        InputManager.ResetAllBindings();

        // Refresh all keybind displays
        foreach (var entry in _keybindEntries)
        {
            if (entry != null)
                entry.Initialize(entry.GetComponent<KeybindEntryUI>() != null
                    ? InputManager.RebindableActions[_keybindEntries.IndexOf(entry)]
                    : null);
        }

        // Just repopulate to refresh everything
        PopulateKeybinds();
    }

    private void OnBackClicked()
    {
        // Cancel any in-progress rebind before leaving
        if (InputManager.IsRebinding)
            InputManager.CancelRebind();

        if (UIManager.HasInstance())
            UIManager.Instance.ShowMainMenu();
    }
}
