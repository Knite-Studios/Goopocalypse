using System;
using OneJS;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

namespace Managers
{
    public partial class AudioManager : MonoSingleton<AudioManager>
    {
        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/AudioManager");
            if (prefab == null) throw new Exception("Missing AudioManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate AudioManager prefab!");

            instance.name = "Managers.AudioManager (Singleton)";
        }

        public AudioMixer audioMixer;

        [EventfulProperty] private float _musicVolume;
        [EventfulProperty] private float _soundFxVolume;

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

        public void SetMusicVolume()
            => audioMixer.SetFloat("Music", Mathf.Log10(MusicVolume) * 20);

        public void SetSoundFxVolume()
            => audioMixer.SetFloat("SoundFx", Mathf.Log10(SoundFxVolume) * 20);
    }
}
