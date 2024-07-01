using System;
using OneJS;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Managers
{
    public partial class SettingsManager : MonoSingleton<SettingsManager>
    {
        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/SettingsManager");
            if (prefab == null) throw new Exception("Missing SettingsManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate SettingsManager prefab!");

            instance.name = "Managers.SettingsManager (Singleton)";
        }

        public AudioMixer audioMixer;

        private AudioSource _audioSource;

        #region JavaScript Accessible

        [EventfulProperty] private float _musicVolume;
        [EventfulProperty] private float _soundFxVolume;
        [EventfulProperty] private DisplayMode _display = DisplayMode.FullScreen;

        #endregion

        protected override void OnAwake()
        {
            OnDisplayChanged += SetDisplayMode;
            OnMusicVolumeChanged += _ => SetMusicVolume();
            OnSoundFxVolumeChanged += _ => SetSoundFxVolume();
        }

        private void Start()
        {
            MusicVolume = PlayerPrefsUtil.MusicVolume;
            SoundFxVolume = PlayerPrefsUtil.SoundFxVolume;
            Display = (DisplayMode)PlayerPrefsUtil.DisplayMode;

            Debug.Log($"Music Volume: {MusicVolume}");
            Debug.Log($"Sound FX Volume: {SoundFxVolume}");
            Debug.Log($"Display Mode: {Display}");

            LoadSettings();
        }

        private void LoadSettings()
        {
            SetMusicVolume();
            SetSoundFxVolume();
            SetDisplayMode(Display);
        }

        #region Methods for Javascript use

        /// <summary>
        /// Convert a volume to a 0-9 index.
        /// This is done so JavaScript float imprecision doesn't matter.
        /// #BlameJavaScript
        /// </summary>
        public int VolumeToIndex(float volume)
        {
            return Mathf.CeilToInt(volume * 10) - 1;
        }

        private void SetMusicVolume()
        {
            audioMixer.SetFloat("Music", MusicVolume == 0 ? -80.0f : Mathf.Log10(MusicVolume) * 20);
            PlayerPrefsUtil.MusicVolume = MusicVolume;
        }

        private void SetSoundFxVolume()
        {
            audioMixer.SetFloat("SoundFx", SoundFxVolume == 0 ? -80.0f : Mathf.Log10(SoundFxVolume) * 20);
            PlayerPrefsUtil.SoundFxVolume = SoundFxVolume;
        }

        /// <summary>
        /// Sets the display mode of the game.
        /// </summary>
        public void SetDisplayMode(DisplayMode mode)
        {
            switch (mode)
            {
                case DisplayMode.FullScreen:
#if UNITY_STANDALONE_WIN
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
#endif
                case DisplayMode.Borderless:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case DisplayMode.Windowed:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                default:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
            }

            PlayerPrefsUtil.DisplayMode = (int)mode;
        }

        #endregion

        public enum DisplayMode
        {
            FullScreen = 0,
            Borderless = 1,
            Windowed = 2,
        }
    }
}
