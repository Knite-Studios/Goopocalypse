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
        [SerializeField] private AudioClip onHoverSound;
        [SerializeField] private AudioClip onClickSound;

        private AudioSource _audioSource;

        #region JavaScript Accessible

        [EventfulProperty] private float _musicVolume;
        [EventfulProperty] private float _soundFxVolume;
        [EventfulProperty] private DisplayMode _display = DisplayMode.FullScreen;

        #endregion

        private void Start()
        {
            MusicVolume = PlayerPrefsUtil.MusicVolume;
            SoundFxVolume = PlayerPrefsUtil.SoundFxVolume;

            LoadVolumeSettings();
        }

        private void LoadVolumeSettings()
        {
            SetMusicVolume();
            SetSoundFxVolume();
        }

        #region Methods for Javascript use

        public void SetMusicVolume()
            => audioMixer.SetFloat("Music", Mathf.Log10(MusicVolume) * 20);

        public void SetSoundFxVolume()
            => audioMixer.SetFloat("SoundFx", Mathf.Log10(SoundFxVolume) * 20);

        public void PlayUIHoverSound()
        {
            if (_audioSource.isPlaying) _audioSource.Stop();
            if (onHoverSound) _audioSource.PlayOneShot(onHoverSound);
        }

        public void PlayUIClickSound()
        {
            if (_audioSource.isPlaying) _audioSource.Stop();
            if (onClickSound) _audioSource.PlayOneShot(onClickSound);
        }

        /// <summary>
        /// Sets the display mode of the game.
        /// </summary>
        public void SetDisplayMode(DisplayMode mode)
        {
            Display = mode;
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
        }

        #endregion

        public enum DisplayMode
        {
            FullScreen,
            Borderless,
            Windowed
        }
    }
}
