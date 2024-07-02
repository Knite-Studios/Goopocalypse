using Managers;
using Mirror;
using UnityEngine;

namespace Entity
{
    public class Orb : NetworkBehaviour
    {
        public long points = 10;
        [SerializeField] private Vector2 force = new (1, 1);

        private Rigidbody2D _rb;
        private bool _isGameOver;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            var randomForce = new Vector2(Random.Range(-force.x, force.x), Random.Range(-force.y, force.y));
            _rb.AddForce(randomForce, ForceMode2D.Impulse);
            _rb.drag = _rb.angularDrag = 0.5f;

            GameManager.OnGameOver += () => _isGameOver = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isGameOver) return;
            if (!other.IsPlayer()) return;
            if (!other.TryGetComponent<BaseEntity>(out var entity)) return;

            WaveManager.Instance.Score += points;

            if (NetworkServer.active)
                NetworkServer.Destroy(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
