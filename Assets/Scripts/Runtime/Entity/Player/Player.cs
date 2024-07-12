using System;
using Attributes;
using Cinemachine;
using Discord;
using Effects;
using JetBrains.Annotations;
using Managers;
using Mirror;
using Systems.Attributes;
using UnityEngine;
using XLua;
using Attribute = Systems.Attributes.Attribute;

namespace Entity.Player
{
    [CSharpCallLua]
    public class Player : BaseEntity
    {
        [TitleHeader("Player Settings")]
        [SyncVar] public PlayerRole playerRole;
        [SerializeField] private CinemachineVirtualCamera virtualCameraPrefab;

        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string Name { get; private set; }

        #region Attribute Getters

        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

        #endregion

        private bool _isDead;
        private CinemachineVirtualCamera _virtualCamera;
        private Vector2 _spawnPosition;

        protected override void Awake()
        {
            base.Awake();

            IsPlayer = true;
        }

        protected virtual void Start()
        {
            InitializeEntityFromLua();
            InitializePlayerConfig();
            InitializePlayerCamera();
        }

        /// <summary>
        /// Loads the player's configuration from the scriptable object.
        /// </summary>
        /// <exception cref="Exception">Thrown when the player role is not found in the map.</exception>
        private void InitializePlayerConfig()
        {
            if (!PlayerRoleMap.Map.TryGetValue(playerRole, out var config))
                throw new Exception($"Missing player config for role: {playerRole}");

            luaScript = config.luaScript;
            Animator.runtimeAnimatorController = config.animatorController;
            SpriteRenderer.sprite = config.sprite;
            SpriteRenderer.sortingOrder = config.sortingOrder;
            Rb.mass = config.mass;
            _spawnPosition = config.spawnPoint;
            transform.position = _spawnPosition;

            DiscordController.Instance.SetSmallImage(playerRole);
        }

        /// <summary>
        /// Creates the player's own camera.
        /// </summary>
        private void InitializePlayerCamera()
        {
            if (isLocalPlayer)
            {
                var playerTransform = transform;
                _virtualCamera = Instantiate(virtualCameraPrefab, playerTransform);
                _virtualCamera.m_Lens.OrthographicSize = this.GetAttributeValue<float>(Attribute.CameraDistance);
                _virtualCamera.Follow = playerTransform;
                _virtualCamera.LookAt = playerTransform;
                _virtualCamera.Priority = 100;
            }
            else if (GameManager.Instance.LocalMultiplayer)
            {
                // We only need 1 camera for local multiplayer.
                if (playerRole is PlayerRole.Buddie) return;

                _virtualCamera = Instantiate(virtualCameraPrefab, transform);
                var targetGroup = FindObjectOfType<CinemachineTargetGroup>();
                if (targetGroup)
                {
                    _virtualCamera.Follow = targetGroup.transform;
                    _virtualCamera.LookAt = targetGroup.transform;

                    var composer = _virtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
                    composer.m_MinimumOrthoSize = this.GetAttributeValue<float>(Attribute.CameraDistance);
                }
            }
        }

        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        protected override void ApplyBaseStats(LuaTable stats)
        {
            base.ApplyBaseStats(stats);

            Name = stats.Get<string>("name");
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
            this.GetOrCreateAttribute(Attribute.CameraDistance, stats.Get<float>("camera_distance"));
        }

        public override void OnDeath()
        {
            if (_isDead) return;

            _isDead = true;
            onDeathEvent?.Invoke();

            Rb.constraints = RigidbodyConstraints2D.FreezeAll;
            if (_virtualCamera) CameraShake.TriggerShake(_virtualCamera);
            Animator.SetTrigger(IsDeadHash);
            Collider.enabled = false;

            base.OnDeath();
            HeartManager.OnPlayerDeath();
        }

        public override void OnDeathAnimation()
        {
            if (HeartManager.Instance.Hearts <= 0)
            {
                Dispose();
            }
            else
            {
                if (NetworkServer.active)
                    NetworkServer.SendToAll(new PlayerRespawnS2CNotify { userId = netId.ToString() });
                else
                    RespawnPlayer();
            }
        }

        /// <summary>
        /// Method called for death sounds.
        /// </summary>
        public override void OnDeathSound()
        {
            if (AudioSource.isPlaying) AudioSource.Stop();
            AudioManager.Instance.PlayOneShot(deathSound, transform.position);
        }

        public void RespawnPlayer()
        {
            _isDead = false;
            Collider.enabled = true;
            transform.Reset(true, true);
            transform.position = _spawnPosition;
            Rb.constraints = RigidbodyConstraints2D.None;
            Rb.velocity = Vector2.zero;
            Rb.angularVelocity = 0;
            Animator.ResetTrigger(IsDeadHash);
            if (_virtualCamera) CameraShake.StopShake();
        }
    }

    [Serializable]
    public struct PlayerSession
    {
        /// <summary>
        /// This is only applicable on the server.
        /// </summary>
        [CanBeNull, NonSerialized] public NetworkConnectionToClient connection;

        public string address, userId;
        [CanBeNull] public Texture2D profileIcon;
        public bool isReady;
    }

    public static class LuaPlayer
    {
        public const string Fwend = "players/fwend.lua";
        public const string Buddie = "players/buddie.lua";
    }
}
