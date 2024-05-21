using Attributes;
using Cinemachine;
using Managers;
using Mirror;
using UnityEngine;

namespace Entity.Player
{
    public class PlayerController : Player
    {
        [TitleHeader("PlayerController Settings")]
        [SerializeField] private PrefabType projectilePrefab; // Temporary for prototype.
        [SerializeField] private GameObject indicator; // Temoprarily used for testing.
        [SerializeField] private float projectileSpawnDistance = 1.0f;
        [SerializeField] private float attackInterval = 2.0f;
        [SerializeField] private CinemachineVirtualCamera virtualCameraPrefab;

        private Rigidbody2D _rigidBody;
        private Vector2 _direction;
        private float _attackTimer;
        private CinemachineVirtualCamera _virtualCamera;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        protected override void Start()
        {
            base.Start();

            if (!isLocalPlayer) return;

            _virtualCamera = Instantiate(virtualCameraPrefab, transform);
            _virtualCamera.Follow = transform;
            _virtualCamera.LookAt = transform;
            _virtualCamera.Priority = 100;
        }

        private void OnDestroy()
        {
            if (!isLocalPlayer) return;

            EntityManager.Instance.CmdRemovePlayerFromTargetGroup(this);
        }

        private void Update()
        {
            if (!isLocalPlayer) return;

            HandleAutoAttack();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            HandleMovement();
        }

        private void HandleMovement()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            if (move != Vector2.zero) _direction = move.normalized;
            // Rotate the indicator based on the direction it's facing.
            indicator.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg);
            var movement = move * (Speed * Time.fixedDeltaTime);

            _rigidBody.MovePosition(_rigidBody.position + movement);
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
    }
}
