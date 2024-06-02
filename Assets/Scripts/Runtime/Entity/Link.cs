using System;
using Entity.Player;
using UnityEngine;

namespace Entity
{
    public class Link : MonoBehaviour
    {
        public float maxDistance = 5.0f;

        private LineRenderer _lineRenderer;
        private BoxCollider2D _collider;
        private Transform _fwend;
        private Transform _buddie;

        private void Start()
        {
            _lineRenderer = gameObject.GetOrAddComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
            _lineRenderer.sortingOrder = 100;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // TODO: Replace later.
            _lineRenderer.startColor = Color.magenta; // TODO: Replace later.
            _lineRenderer.endColor = Color.yellow; // TODO: Replace later.

            _collider = gameObject.GetOrAddComponent<BoxCollider2D>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        private void Update()
        {
            if (!_fwend || !_buddie)
            {
                var findPlayers = FindObjectsOfType<MonoPlayerController>();
                foreach (var player in findPlayers)
                {
                    switch (player.playerRole)
                    {
                        case PlayerRole.Fwend:
                            _fwend = player.transform;
                            break;
                        case PlayerRole.Buddie:
                            _buddie = player.transform;
                            break;
                    }
                }

                return;
            }

            if (Vector2.Distance(_fwend.position, _buddie.position) <= maxDistance)
            {
                if (!_collider.enabled) _collider.enabled = true;

                // Connect the players with a line.
                _lineRenderer.SetPosition(0, _fwend.position);
                _lineRenderer.SetPosition(1, _buddie.position);

                // Get the midpoint between the players and adjust the collider size dynamically.
                var midpoint = (_fwend.position + _buddie.position) / 2;
                transform.position = midpoint;
                var distance = Vector2.Distance(_fwend.position, _buddie.position);
                _collider.size = new Vector2(distance, 0.3f);
                transform.right = (_buddie.position - _fwend.position).normalized;
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
