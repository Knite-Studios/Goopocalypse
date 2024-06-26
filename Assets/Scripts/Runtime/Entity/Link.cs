using Entity.Player;
using JetBrains.Annotations;
using Mirror;
using UnityEngine;

namespace Entity
{
    public class Link : NetworkBehaviour
    {
        public Transform fwend;
        public Transform buddie;
        [SerializeField, CanBeNull] Material lineMaterial;
        [SerializeField] Color startColor = Color.magenta;
        [SerializeField] Color endColor = Color.yellow;
        [SerializeField] public float maxDistance = 5.0f;

        private LineRenderer _lineRenderer;
        private BoxCollider2D _collider;

        private void Start()
        {
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.sortingOrder = 0;
            _lineRenderer.material = lineMaterial == null
                ? new Material(Shader.Find("Sprites/Default"))
                : lineMaterial;
            _lineRenderer.startColor = startColor;
            _lineRenderer.endColor = endColor;

            _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        private void Update()
        {
            if (!fwend || !buddie)
            {
                var findPlayers = FindObjectsOfType<PlayerController>();
                foreach (var player in findPlayers)
                {
                    switch (player.playerRole)
                    {
                        case PlayerRole.Fwend:
                            fwend = player.transform;
                            break;
                        case PlayerRole.Buddie:
                            buddie = player.transform;
                            break;
                    }
                }

                return;
            }

            if (Vector2.Distance(fwend.position, buddie.position) <= maxDistance)
            {
                if (!_collider.enabled) _collider.enabled = true;

                // Connect the players with a line.
                _lineRenderer.SetPosition(0, fwend.position);
                _lineRenderer.SetPosition(1, buddie.position);

                // Get the midpoint between the players and adjust the collider size dynamically.
                var midpoint = (fwend.position + buddie.position) / 2;
                transform.position = midpoint;
                var distance = Vector2.Distance(fwend.position, buddie.position);
                _collider.size = new Vector2(distance, 0.3f);
                transform.right = (buddie.position - fwend.position).normalized;
            }
            else
            {
                _lineRenderer.SetPosition(0, Vector2.zero);
                _lineRenderer.SetPosition(1, Vector2.zero);
                if (_collider.enabled) _collider.enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.IsPlayer()) return;
            if (!other.TryGetComponent(out BaseEntity entity)) return;

            // Apply full damage to the entity.
            entity.CmdDamage(entity.CurrentHealth, true);
        }
    }
}
