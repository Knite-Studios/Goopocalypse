using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Wires the settings UI (sliders, display dropdown, back button) to SettingsManager.
    /// All UI must be built in the scene; assign references in the Inspector.
    /// </summary>
    public class SettingsMenuController : MonoBehaviour
    {
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TMP_Dropdown displayDropdown;
        [SerializeField] private Button backButton;

        private SettingsManager _settings;

        private void Start()
        {
            if (!SettingsManager.HasInstance())
            {
                Debug.LogWarning("[SettingsMenuController] SettingsManager not found.");
                return;
            }

            _settings = SettingsManager.Instance;

            if (musicSlider != null)
            {
                musicSlider.minValue = 0f;
                musicSlider.maxValue = 1f;
                musicSlider.value = _settings.MusicVolume;
                musicSlider.onValueChanged.AddListener(v => _settings.MusicVolume = v);
            }

            if (sfxSlider != null)
            {
                sfxSlider.minValue = 0f;
                sfxSlider.maxValue = 1f;
                sfxSlider.value = _settings.SoundFxVolume;
                sfxSlider.onValueChanged.AddListener(v => _settings.SoundFxVolume = v);
            }

            if (displayDropdown != null)
            {
                displayDropdown.ClearOptions();
                displayDropdown.options.Add(new TMP_Dropdown.OptionData("Fullscreen"));
                displayDropdown.options.Add(new TMP_Dropdown.OptionData("Borderless"));
                displayDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed"));
                displayDropdown.value = (int)_settings.Display;
                displayDropdown.onValueChanged.AddListener(OnDisplayChanged);
            }

            if (backButton != null)
                backButton.onClick.AddListener(CloseSettings);
        }

        private void OnDisplayChanged(int index)
        {
            if (_settings == null) return;
            _settings.Display = (SettingsManager.DisplayMode)Mathf.Clamp(index, 0, 2);
        }

        private void CloseSettings()
        {
            var menu = FindObjectOfType<MenuController>();
            if (menu != null)
                menu.CloseSettings();
        }

        private void OnDestroy()
        {
            if (musicSlider != null)
                musicSlider.onValueChanged.RemoveAllListeners();
            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveAllListeners();
            if (displayDropdown != null)
                displayDropdown.onValueChanged.RemoveAllListeners();
            if (backButton != null)
                backButton.onClick.RemoveListener(CloseSettings);
        }
    }
}
