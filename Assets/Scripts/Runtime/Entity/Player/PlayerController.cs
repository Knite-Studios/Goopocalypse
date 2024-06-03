using Attributes;
using Cinemachine;
using Managers;
using Runtime.World;
using UnityEngine;

namespace Entity.Player
{
    public class PlayerController : Player
    {
        [TitleHeader("PlayerController Settings")]
        [SerializeField] private GameObject indicator; // Temporarily used for testing.
        [SerializeField] private CinemachineVirtualCamera virtualCameraPrefab;

        private Vector2 _direction;
        private float _attackTimer;
        private CinemachineVirtualCamera _virtualCamera;

        protected override void Awake()
        {
            base.Awake();

            GameManager.OnWorldGenerated += OnWorldGenerated;
        }

        protected override void Start()
        {
            base.Start();

            if (!isLocalPlayer) return;

            // Create the player's virtual camera.
            var playerTransform = transform;

            _virtualCamera = Instantiate(virtualCameraPrefab, playerTransform);
            _virtualCamera.Follow = playerTransform;
            _virtualCamera.LookAt = playerTransform;
            _virtualCamera.Priority = 100;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            HandleMovement();
        }

        private void OnWorldGenerated(World world)
        {
            transform.SetPositionAndRotation(world.spawnPoint, Quaternion.identity);
        }

        private void HandleMovement()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            if (move != Vector2.zero) _direction = move.normalized;

            // Rotate the indicator based on the direction it's facing.
            indicator.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg);

            // Flip the sprite based on the direction it's facing.
            SpriteRenderer.flipX = _direction.x < 0;

            var movement = move * (Speed * Time.fixedDeltaTime);
            Rb.MovePosition(Rb.position + movement);
        }
    }
}
