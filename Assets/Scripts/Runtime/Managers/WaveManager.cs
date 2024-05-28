using System.Collections;
using Mirror;
using OneJS;
using Runtime.World;
using UnityEngine;

namespace Managers
{
    public partial class WaveManager : MonoSingleton<WaveManager>
    {
        [EventfulProperty] private int _waveCount = 1;
        [EventfulProperty] private long _matchTimer;

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
            GameManager.OnGameStart += OnGameStart;
        }

        /// <summary>
        /// Invoked when the game starts.
        /// </summary>
        private void OnGameStart()
        {
            if (!NetworkServer.activeHost) return;

            _gameRunning = true;
            WaveCount = 1;

            StartCoroutine(CountTimer());
        }

        /// <summary>
        /// Called once per second.
        /// </summary>
        private void Tick()
        {
            if (_matchTimer % spawnThreshold == 0)
            {
                SpawnWave();
                WaveCount++;
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
                MatchTimer++;

                Tick();
            }
        }

        /// <summary>
        /// Creates a wave of enemies.
        /// TODO: Add a correct entity count.
        /// </summary>
        public void SpawnWave() => GameManager.OnWaveSpawn?.Invoke(_waveCount);
    }
}
