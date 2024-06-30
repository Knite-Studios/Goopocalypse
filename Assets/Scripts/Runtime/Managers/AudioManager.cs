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
            tempAudioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(
                type is AudioType.SoundFx ? "SoundFx" : "Music")[0];

            // If the audio should play based on proximity, adjust the volume accordingly.
            if (proximity)
            {
                var player = EntityManager.Instance.GetLocalPlayer();
                var distance = Vector3.Distance(position, player.transform.position);
                var volume = Mathf.Clamp01(1 - distance / maxDistance);
                tempAudioSource.volume = volume;
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
