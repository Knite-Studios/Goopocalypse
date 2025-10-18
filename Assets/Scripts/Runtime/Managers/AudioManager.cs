using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class AudioManager : MonoSingleton<AudioManager>
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
        [SerializeField] private AudioClip onHoverSound;
        [SerializeField] private AudioClip onClickSound;

        private AudioSource _audioSource;

        protected override void OnAwake()
        {
            // Try to get existing AudioSource, or add one if missing
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                Debug.LogWarning("AudioSource component was missing - added automatically");
            }

            // Configure the AudioSource
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
        }

        public void PlayUIHoverSound()
        {
            if (_audioSource == null || onHoverSound == null) return;

            if (_audioSource.isPlaying) _audioSource.Stop();
            _audioSource.PlayOneShot(onHoverSound);
        }

        public void PlayUIClickSound()
        {
            if (_audioSource == null || onClickSound == null) return;

            if (_audioSource.isPlaying) _audioSource.Stop();
            _audioSource.PlayOneShot(onClickSound);
        }

        /// <summary>
        /// Plays a sound effect at the specified position.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="position">The position to play the audio at.</param>
        /// <param name="proximity">Whether the audio should play based on proximity.</param>
        /// <param name="maxDistance">The maximum distance the audio can be heard from.</param>
        /// <param name="type">The type of audio to play (SoundFx or Music).</param>
        public void PlayOneShot(
            AudioClip clip,
            Vector3 position,
            bool proximity = true,
            float maxDistance = 10.0f,
            AudioType type = AudioType.SoundFx)
        {
            if (!clip) return;

            // Create a temporary game object to play the audio.
            var temp = new GameObject("TempAudio")
            {
                transform =
                {
                    position = position,
                    parent = transform
                }
            };

            // Add and configure the audio source.
            var tempAudioSource = temp.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;

            // Set the audio mixer group
            if (audioMixer != null)
            {
                var groups = audioMixer.FindMatchingGroups(
                    type is AudioType.SoundFx ? "SoundFx" : "Music");
                if (groups != null && groups.Length > 0)
                {
                    tempAudioSource.outputAudioMixerGroup = groups[0];
                }
            }

            // If the audio should play based on proximity, adjust the volume accordingly.
            if (proximity)
            {
                var player = EntityManager.Instance?.GetLocalPlayer();
                // Play with no volume adjustment if no player is found.
                if (!player)
                {
                    tempAudioSource.volume = 1.0f;
                }
                else
                {
                    var distance = Vector3.Distance(position, player.transform.position);
                    var volume = Mathf.Clamp01(1 - distance / maxDistance);
                    tempAudioSource.volume = volume;
                }
            }
            else
            {
                tempAudioSource.volume = 1.0f;
            }

            // Play the audio.
            tempAudioSource.Play();

            // Destroy after the clip has finished playing.
            Destroy(temp, clip.length);
        }

        public enum AudioType
        {
            Music,
            SoundFx
        }
    }
}
