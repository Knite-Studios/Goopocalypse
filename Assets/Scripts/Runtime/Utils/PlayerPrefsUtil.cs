using UnityEngine;

namespace Utils
{
    public static class PlayerPrefsUtil
    {
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat("MusicVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
            }
        }

        public static float SoundFxVolume
        {
            get => PlayerPrefs.GetFloat("SoundFxVolume", 1.0f);
            set
            {
                PlayerPrefs.SetFloat("SoundFxVolume", value);
                PlayerPrefs.Save();
            }
        }

        public static int DisplayMode
        {
            get => PlayerPrefs.GetInt("DisplayMode", 0);
            set
            {
                PlayerPrefs.SetInt("DisplayMode", value);
                PlayerPrefs.Save();
            }
        }

        public static bool FirstLocal
        {
            get => PlayerPrefs.GetInt("FirstLocal", 1) == 1;
            set
            {
                PlayerPrefs.SetInt("FirstLocal", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static bool FirstCoop
        {
            get => PlayerPrefs.GetInt("FirstCoop", 1) == 1;
            set
            {
                PlayerPrefs.SetInt("FirstCoop", value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Stores input binding overrides as JSON from InputActionAsset.SaveBindingOverridesAsJson().
        /// </summary>
        public static string KeybindOverrides
        {
            get => PlayerPrefs.GetString("KeybindOverrides", string.Empty);
            set
            {
                PlayerPrefs.SetString("KeybindOverrides", value);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Clears all keybind overrides, resetting to defaults.
        /// </summary>
        public static void ClearKeybindOverrides()
        {
            PlayerPrefs.DeleteKey("KeybindOverrides");
            PlayerPrefs.Save();
        }
    }
}
