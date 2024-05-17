using System.Collections;
using System.Collections.Generic;
using Entity;
using Mirror;
using UnityEditor;
using UnityEngine;

namespace Managers
{
    public class WaveManager : NetworkSingleton<WaveManager>
    {
        [SyncVar] private int _waveCount = 1;
        [SyncVar] private long _matchTimer = 0; // Seconds elapsed since match start.

        private bool _isWaveActive;
        private int _enemiesRemaining;
        private List<GameObject> _aliveEnemies = new();

        [SerializeField] private float distance = 2f;
        private Camera _mainCamera;

        protected override void OnAwake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            GameManager.OnGameStart += OnGameStart;
            Debug.Log("Initialized wave manager.");
        }

        /// <summary>
        /// Called when the game is requested to start.
        /// </summary>
        [Server]
        private void OnGameStart()
        {
            Debug.Log("Game started");

            _isWaveActive = true;

            StartCoroutine(SpawnEnemies());
        }

        /// <summary>
        /// Handles spawning enemies during a wave.
        /// </summary>
        private IEnumerator SpawnEnemies()
        {
            while (_isWaveActive)
            {
                yield return new WaitForSeconds(3);

                // Pick a random point within the host's camera's view.
                var radius = _mainCamera.orthographicSize * distance;
                var point = Random.insideUnitCircle.normalized * radius;

                var enemy = new GameObject("Enemy")
                {
                    transform =
                    {
                        position = point,
                        localScale = new Vector3(5, 5, 5)
                    },
                };
                enemy.AddComponent<Enemy>();
                var renderer = enemy.AddComponent<SpriteRenderer>();
                renderer.color = Color.red;
                renderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                
                NetworkServer.Spawn(enemy);
            }
        }
    }
}
