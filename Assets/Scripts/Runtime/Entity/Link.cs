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
        [SerializeField] public float maxDistance = 5.0f;

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
        private bool _isConnected;

        private void Awake()
        {
            _audioSource = gameObject.GetOrAddComponent<AudioSource>();
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
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
            if (!fwend || !buddie) return;

            if (Vector2.Distance(fwend.position, buddie.position) <= maxDistance)
            {
                if (!_isConnected)
                {
                    _isConnected = true;
                    onLinkConnected?.Invoke();
                }

                if (!_collider.enabled) _collider.enabled = true;

                var fwendPos = GetSpriteMiddlePoint(fwend);
                var buddiePos = GetSpriteMiddlePoint(buddie);

                // Connect the players with a line.
                _lineRenderer.SetPosition(0, fwendPos);
                _lineRenderer.SetPosition(1, buddiePos);

                // Get the midpoint between the players and adjust the collider size dynamically.
                var midpoint = (fwendPos + buddiePos) / 2;
                transform.position = midpoint;
                var distance = Vector2.Distance(fwendPos, buddiePos);
                _collider.size = new Vector2(distance, 0.3f);
                transform.right = (fwendPos - buddiePos).normalized;
            }
            else
            {
                if (_isConnected)
                {
                    _isConnected = false;
                    onLinkBreak?.Invoke();
                }

                _lineRenderer.SetPosition(0, Vector2.zero);
                _lineRenderer.SetPosition(1, Vector2.zero);
                if (_collider.enabled) _collider.enabled = false;
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

        private Vector2 GetSpriteMiddlePoint(Transform playerTransform)
        {
            var spriteRenderer = playerTransform.GetComponent<SpriteRenderer>();
            if (!spriteRenderer) return playerTransform.position;

            var bounds = spriteRenderer.sprite.bounds;
            return playerTransform.position + bounds.center;
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
