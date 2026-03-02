using System;
using System.Collections;
using Attributes;
using Cinemachine;
using Discord;
using Effects;
using JetBrains.Annotations;
using Managers;
using Mirror;
using Scriptable;
using Systems.Attributes;
using UnityEngine;
using Attribute = Systems.Attributes.Attribute;

namespace Entity.Player
{
    public class Player : BaseEntity
    {
        [TitleHeader("Player Settings")]
        [SyncVar] public PlayerRole playerRole;
        [SerializeField] private CinemachineVirtualCamera virtualCameraPrefab;

        public string Name { get; private set; }

        #region Attribute Getters

        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

        #endregion

        protected bool _isDead;
        private CinemachineVirtualCamera _virtualCamera;
        private Vector2 _spawnPosition;

        protected override void Awake()
        {
            base.Awake();

            IsPlayer = true;
        }

        protected virtual void Start()
        {
            InitializePlayerConfig();
            InitializePlayerCamera();
        }

        /// <summary>
        /// Loads visual config and stats from the PlayerConfig ScriptableObject.
        /// </summary>
        private void InitializePlayerConfig()
        {
            if (!PlayerRoleMap.Map.TryGetValue(playerRole, out var config))
                throw new Exception($"Missing player config for role: {playerRole}");

            Animator.runtimeAnimatorController = config.animatorController;
            SpriteRenderer.sprite = config.sprite;
            SpriteRenderer.sortingOrder = config.sortingOrder;
            Rb.mass = config.mass;
            _spawnPosition = config.spawnPoint;
            transform.position = _spawnPosition;

            ApplyPlayerStats(config);

            DiscordController.Instance.SetSmallImage(playerRole);
        }

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
        /// Loads player stats from a PlayerConfig ScriptableObject into the Attribute system.
        /// </summary>
        private void ApplyPlayerStats(PlayerConfig config)
        {
            this.GetOrCreateAttribute(Attribute.MaxHealth, config.maxHealth);
            this.GetOrCreateAttribute(Attribute.Speed, config.speed);
            this.GetOrCreateAttribute(Attribute.Armor, config.armor);
            this.GetOrCreateAttribute(Attribute.Stamina, config.stamina);
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, config.areaOfEffect);
            this.GetOrCreateAttribute(Attribute.CameraDistance, config.cameraDistance);

            Name = config.playerName;
            CurrentHealth = MaxHealth;
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
            StartCoroutine(DeathAnimation());
        }

        IEnumerator DeathAnimation()
        {
            yield return null;
            var animationDuration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationDuration);
            OnDeathAnimation();
        }

        public override void OnDeathAnimation()
        {
            // One death = game over. Invoke on this machine first so host/server also stops (both players freeze).
            GameManager.OnGameOver?.Invoke();

            if (!GameManager.Instance.LocalMultiplayer && NetworkServer.active)
                NetworkServer.SendToAll(new GameOverS2CNotify());

            Dispose();
        }

        /// <summary>
        /// Method called for death sounds.
        /// </summary>
        public override void OnDeathSound()
        {
            if (AudioSource.isPlaying) AudioSource.Stop();
            AudioManager.Instance.PlayOneShot(deathSound, transform.position);
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

}
