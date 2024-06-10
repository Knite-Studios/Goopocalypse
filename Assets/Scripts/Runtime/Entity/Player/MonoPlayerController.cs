using Managers;
using UnityEngine;

namespace Entity.Player
{
    public class MonoPlayerController : MonoBehaviour
    {
        public PlayerRole playerRole;
        public float moveSpeed = 10.0f;

        private Rigidbody2D _rb;
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();

        }
        private void FixedUpdate()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            var movement = move * (moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(_rb.position + movement);

            bool isMoving = move != Vector2.zero;
            _animator.SetBool("IsMoving", isMoving);

            if (move.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1); 
            }
            else if (move.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1); 
            }
        }
    }
}
