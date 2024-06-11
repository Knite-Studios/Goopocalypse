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
        [SerializeField] private CinemachineVirtualCamera virtualCameraPrefab;

        public bool IsMoving { get; private set; }

        private Vector2 _direction;
        private float _attackTimer;
        private CinemachineVirtualCamera _virtualCamera;

        private static readonly int Moving = Animator.StringToHash("IsMoving");

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
            IsMoving = move != Vector2.zero;
            Animator.SetBool(Moving, IsMoving);
            if (move != Vector2.zero) _direction = move.normalized;

            // Flip the sprite based on the direction it's facing.
            SpriteRenderer.flipX = _direction.x < 0;

            var movement = move * (Speed * Time.fixedDeltaTime);
            Rb.MovePosition(Rb.position + movement);
        }
    }
}
