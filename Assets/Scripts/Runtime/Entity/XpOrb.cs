using Managers;
using Mirror;
using UnityEngine;

namespace Entity
{
    /// <summary>
    /// XP orb: on pickup adds XP to XpManager. Spawned by enemies on death (alongside currency orb).
    /// </summary>
    public class XpOrb : NetworkBehaviour
    {
        public int xp = 5;
        [SerializeField] private Vector2 force = new Vector2(1, 1);

        private Rigidbody2D _rb;
        private bool _isGameOver;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb)
            {
                var randomForce = new Vector2(Random.Range(-force.x, force.x), Random.Range(-force.y, force.y));
                _rb.AddForce(randomForce, ForceMode2D.Impulse);
                _rb.drag = _rb.angularDrag = 0.5f;
            }
            if (GameManager.Instance != null)
                GameManager.OnGameOver += () => _isGameOver = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isGameOver) return;
            if (!other.IsPlayer()) return;

            if (XpManager.Instance != null)
                XpManager.Instance.AddXp(xp);

            if (NetworkServer.active)
                NetworkServer.Destroy(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
