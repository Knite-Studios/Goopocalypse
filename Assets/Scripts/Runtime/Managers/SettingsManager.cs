using System;
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

        #region Unity Native Properties with Events

        private float _musicVolume;
        private float _soundFxVolume;
        private DisplayMode _display = DisplayMode.FullScreen;

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                if (_musicVolume != value)
                {
                    _musicVolume = value;
                    OnMusicVolumeChanged?.Invoke(value);
                }
            }
        }

        public float SoundFxVolume
        {
            get => _soundFxVolume;
            set
            {
                if (_soundFxVolume != value)
                {
                    _soundFxVolume = value;
                    OnSoundFxVolumeChanged?.Invoke(value);
                }
            }
        }

        public DisplayMode Display
        {
            get => _display;
            set
            {
                if (_display != value)
                {
                    _display = value;
                    OnDisplayChanged?.Invoke(value);
                }
            }
        }

        // Events for property changes
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSoundFxVolumeChanged;
        public event Action<DisplayMode> OnDisplayChanged;

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

            LoadSettings();
        }

        private void LoadSettings()
        {
            SetMusicVolume();
            SetSoundFxVolume();
            SetDisplayMode(Display);
        }

        #region Methods for external use

        /// <summary>
        /// Convert a volume to a 0-9 index.
        /// This is done so float imprecision doesn't matter.
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
