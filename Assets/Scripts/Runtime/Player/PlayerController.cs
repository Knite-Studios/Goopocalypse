using Managers;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerController : NetworkBehaviour
    {
        public float speed = 0.15f;

        [SerializeField] private PrefabType projectilePrefab; // Temporary for prototype.
        [SerializeField] private GameObject indicator; // Temoprarily used for testing.
        [SerializeField] private float projectileSpawnDistance = 1.0f;
        [SerializeField] private float attackInterval = 2.0f;

        private Rigidbody2D _rigidBody;
        private Vector2 _direction;
        private float _attackTimer;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (isClient)
            {
                Camera.main!.transform.SetParent(transform);
            }
        }

        private void Update()
        {
            if (!isLocalPlayer) return;

            HandleMovement();
            HandleAutoAttack();
        }

        private void HandleMovement()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            if (move != Vector2.zero) _direction = move.normalized;
            // Rotate the indicator based on the direction it's facing.
            indicator.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg);
            var movement = new Vector3(move.x, move.y, 0) * speed;

            _rigidBody.MovePosition(transform.position + movement);
        }

        private void HandleAutoAttack()
        {
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0.0f)
            {
                _attackTimer = attackInterval;
                HandleAttack();
            }
        }

        private void HandleAttack()
        {
            // if (InputManager.Attack.WasReleasedThisFrame())
            // {
            //     var mousePos = Camera.main!.ScreenToWorldPoint(InputManager.Mouse.ReadValue<Vector2>());
            //     mousePos.z = 0;
            //
            //     var direction = (mousePos - transform.position).normalized;
            //     var spawnPosition = transform.position + (direction * projectileSpawnDistance);
            //
            //     var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //     var rotation = Quaternion.Euler(0, 0, angle);
            //
            //     CmdSpawnProjectile(spawnPosition, rotation);
            // }

            var spawnPos = transform.position.Add(_direction * projectileSpawnDistance);

            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(0, 0, angle);

            CmdSpawnProjectile(projectilePrefab, spawnPos, rotation);
        }

        [Command]
        private void CmdSpawnProjectile(PrefabType projectileType, Vector3 position, Quaternion rotation)
        {
            var projectile = PrefabManager.Create(projectileType);
            projectile.transform.SetPositionAndRotation(position, rotation);
            NetworkServer.Spawn(projectile);
        }

        [ClientRpc]
        public void RpcTakeDamage(int damage)
        {
            Debug.Log($"Player took {damage} damage!");
        }
    }
}
