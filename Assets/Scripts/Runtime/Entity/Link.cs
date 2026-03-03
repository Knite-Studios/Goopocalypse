using Attributes;
using Entity.Enemies;
using Entity.Player;
using JetBrains.Annotations;
using Managers;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Entity
{
    public class Link : NetworkBehaviour
    {
        [TitleHeader("Link Settings")]
        public Transform fwend;
        public Transform buddie;
        [SerializeField, CanBeNull] Material lineMaterial;
        [SerializeField] Color startColor = Color.magenta;
        [SerializeField] Color endColor = Color.yellow;
        [Tooltip("Base link length. Upgrades add to this via UpgradeManager.")]
        [SerializeField] public float baseMaxDistance = 5.0f;

        /// <summary> Effective max distance (base + upgrade bonus). Used for link check and visuals. </summary>
        public float maxDistance => baseMaxDistance + (Managers.UpgradeManager.HasInstance() ? Managers.UpgradeManager.Instance.GetLinkLengthBonus() : 0f);

        [TitleHeader("Audio Settings")]
        [SerializeField] AudioClip linkConnected;
        [SerializeField] AudioClip linkBreak;
        [SerializeField] AudioClip linkHit;

        [TitleHeader("Events")]
        public UnityEvent onLinkConnected;
        public UnityEvent onLinkBreak;

        private LineRenderer _lineRenderer;
        private BoxCollider2D _collider;
        private AudioSource _audioSource;
        [SyncVar] private bool _isConnected;
        /// <summary> Used when LocalMultiplayer (no server) so link state still runs. </summary>
        private bool _isConnectedLocal;

        private BaseEntity _fwendEntity, _buddieEntity;

        /// <summary> True when link is active. Uses synced value on server/clients, local value in LocalMultiplayer. </summary>
        private bool IsConnected => (isServer || GameManager.Instance.LocalMultiplayer) ? _isConnectedLocal : _isConnected;

        private void Awake()
        {
            _audioSource = gameObject.GetOrAddComponent<AudioSource>();
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _fwendEntity = fwend.GetComponent<BaseEntity>();
            _buddieEntity = buddie.GetComponent<BaseEntity>();

            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = 0.8f;
            _lineRenderer.endWidth = 0.8f;
            _lineRenderer.sortingOrder = 0;
            _lineRenderer.material = !lineMaterial
                ? new Material(Shader.Find("Sprites/Default"))
                : lineMaterial;
            _lineRenderer.startColor = startColor;
            _lineRenderer.endColor = endColor;

            _collider.isTrigger = true;
            _collider.enabled = false;

            FindPlayers();
        }

        private void OnEnable()
            => GameManager.OnGameOver += DestroyLink;

        private void OnDisable()
            => GameManager.OnGameOver -= DestroyLink;

        private void Update()
        {
            // Only the server (or local multiplayer host) controls link state and light effects.
            if (!isServer && !GameManager.Instance.LocalMultiplayer) return;
            if (!fwend || !buddie) return;

            var distance = Vector2.Distance(fwend.position, buddie.position);
            var shouldBeLinked = distance <= maxDistance;

            if (shouldBeLinked != IsConnected)
            {
                SetLinked(shouldBeLinked);
            }

            if (IsConnected)
            {
                if (!_collider.enabled) _collider.enabled = true;

                var fwendPos = _fwendEntity.GetSpriteMiddlePoint();
                var buddiePos = _buddieEntity.GetSpriteMiddlePoint();

                // Connect the players with a line.
                _lineRenderer.SetPosition(0, fwendPos);
                _lineRenderer.SetPosition(1, buddiePos);

                // Get the midpoint between the players and adjust the collider size dynamically.
                var midpoint = (fwendPos + buddiePos) / 2;
                transform.position = midpoint;
                _collider.size = new Vector2(distance, 0.3f);
                transform.right = (fwendPos - buddiePos).normalized;

                //Change Color based on Distance
                float threshold = 0.5f * maxDistance;
                Color currentColor;
                if (distance > threshold)
                {
                    float t = (distance - threshold) / (maxDistance - threshold);
                    currentColor = Color.Lerp(Color.green, Color.red, t);
                }
                else
                {
                    currentColor = Color.green;
                }
                _lineRenderer.startColor = currentColor;
                _lineRenderer.endColor = currentColor;
                _lineRenderer.material.SetColor("_TintColor", currentColor);
            }
            else
            {
                // Charge the shared ultimate bar only when players are SEPARATED (not linked).
                if (UltimateManager.Instance)
                {
                    if (NetworkServer.active)
                        UltimateManager.Instance.AddTimeCharge(Time.deltaTime);
                    else if (GameManager.Instance.LocalMultiplayer)
                        UltimateManager.Instance.AddTimeChargeLocal(Time.deltaTime);
                }

                _lineRenderer.SetPosition(0, Vector2.zero);
                _lineRenderer.SetPosition(1, Vector2.zero);
                if (_collider.enabled) _collider.enabled = false;

                //Reset Color when disconnected
                _lineRenderer.material.SetColor("_TintColor", Color.green);
            }
        }

        private void SetLinked(bool linked)
        {
            if (!isServer && !GameManager.Instance.LocalMultiplayer) return;

            _isConnectedLocal = linked;
            if (isServer)
                _isConnected = linked;

            if (linked)
            {
                onLinkConnected?.Invoke();
            }
            else
            {
                onLinkBreak?.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.IsPlayer()) return;
            if (!other.TryGetComponent(out Enemy entity)) return;

            AudioManager.Instance.PlayOneShot(linkHit, entity.transform.position);

            // Apply full damage to the entity.
            if (GameManager.Instance.LocalMultiplayer)
                entity.OnDeath();
            else
                entity.Damage(entity.CurrentHealth, true);
        }

        private void FindPlayers()
        {
            var players = EntityManager.Instance.GetPlayers();
            foreach (var player in players)
            {
                switch (player.playerRole)
                {
                    case PlayerRole.Fwend:
                        fwend = player.transform;
                        player.onDeathEvent.AddListener(OnPlayerDeath);
                        break;
                    case PlayerRole.Buddie:
                        buddie = player.transform;
                        player.onDeathEvent.AddListener(OnPlayerDeath);
                        break;
                }
            }
        }

        private void OnPlayerDeath()
        {
            DestroyLink();
        }

        private void DestroyLink()
        {
            if (fwend) fwend.GetComponent<PlayerController>().onDeathEvent.RemoveListener(OnPlayerDeath);
            if (buddie) buddie.GetComponent<PlayerController>().onDeathEvent.RemoveListener(OnPlayerDeath);

            if (isServer)
                NetworkServer.Destroy(gameObject);
            else
                Destroy(gameObject);
        }

        public void OnConnect()
        {
            if (_audioSource.isPlaying) _audioSource.Stop();

            if (linkConnected) _audioSource.PlayOneShot(linkConnected);
            // Spawn VFX at line renderer position 0 and 1 or GetSpriteMiddlePoint of players
        }

        public void OnBreak()
        {
            if (_audioSource.isPlaying) _audioSource.Stop();

            if (linkConnected) _audioSource.PlayOneShot(linkBreak);
            // Spawn VFX at line renderer position 0 and 1 or GetSpriteMiddlePoint of players
        }
    }
}
