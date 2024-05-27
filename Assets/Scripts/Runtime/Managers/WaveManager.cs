using System.Collections;
using Mirror;
using Runtime.World;
using UnityEngine;

namespace Managers
{
    public class WaveManager : MonoSingleton<WaveManager>
    {
        [ReadOnly] public int waveCount = 1;
        [ReadOnly] public long matchTimer;

        [Tooltip("The amount of seconds it takes to spawn a wave.")]
        public int spawnThreshold = 30;

        public World World
        {
            get
            {
                if (_world == null) _world = FindObjectOfType<World>();
                return _world;
            }
        }

        private World _world;
        private bool _gameRunning;

        private void Start()
        {
            if (!NetworkServer.activeHost) return;

            GameManager.OnGameStart += OnGameStart;
        }

        /// <summary>
        /// Invoked when the game starts.
        /// </summary>
        private void OnGameStart()
        {
            _gameRunning = true;
            waveCount = 1;

            StartCoroutine(CountTimer());
        }

        /// <summary>
        /// Called once per second.
        /// </summary>
        private void Tick()
        {
            if (matchTimer % spawnThreshold == 0)
            {
                SpawnWave();
                waveCount++;
            }
        }

        /// <summary>
        /// Counts the time elapsed since the game started.
        /// </summary>
        private IEnumerator CountTimer()
        {
            while (_gameRunning)
            {
                yield return new WaitForSeconds(1);
                matchTimer++;

                Tick();
            }
        }

        /// <summary>
        /// Creates a wave of enemies.
        /// TODO: Add a correct entity count.
        /// </summary>
        public void SpawnWave() => GameManager.OnWaveSpawn?.Invoke(waveCount);
    }
}
