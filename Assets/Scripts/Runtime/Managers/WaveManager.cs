using System.Collections;
using Mirror;
using OneJS;
using Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public partial class WaveManager : MonoSingleton<WaveManager>
    {
        [EventfulProperty] private int _waveCount = 1;
        [EventfulProperty] private long _matchTimer;

        [EventfulProperty] private long _score;

        [Tooltip("The amount of seconds it takes to spawn a wave.")]
        public int spawnThreshold = 4;

        private bool _gameRunning;

        private void Start()
        {
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnGameOver += () => _gameRunning = false;
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
            _waveCount = 1;
            _matchTimer = 0;
            _score = 0;

            StopAllCoroutines();
        }

        #region Packet Handlers

        /// <summary>
        /// Invoked when the server notifies the clients of the current wave info.
        /// </summary>
        public static void OnWaveInfo(WaveInfoS2CNotify notify)
        {
            // Do not run if we are the host.
            if (NetworkServer.activeHost) return;

            Instance.WaveCount = notify.wave;
            Instance.MatchTimer = notify.timer;
        }

        public static void OnScoreUpdate(ScoreUpdateS2CNotify notify)
        {
            // Do not run if we are the host.
            if (NetworkServer.activeHost) return;

            Instance.Score = notify.score;
        }

        #endregion

        /// <summary>
        /// Invoked when the game starts.
        /// </summary>
        private void OnGameStart()
        {
            // Do not run if we are a client.
            if (!NetworkServer.activeHost && !GameManager.Instance.LocalMultiplayer) return;

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

            // Broadcast wave info.
            NetworkServer.SendToAll(new WaveInfoS2CNotify
                { wave = WaveCount, timer = MatchTimer });
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

        public void AddScore(long points)
        {
            Score += points;
            if (NetworkServer.active)
                NetworkServer.SendToAll(new ScoreUpdateS2CNotify { score = Score });
        }
    }
}
