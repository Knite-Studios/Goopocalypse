using Managers;
using UnityEngine;

namespace Entity.Player
{
    public class MonoPlayerController : MonoBehaviour
    {
        public float moveSpeed = 10.0f;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
        private void FixedUpdate()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            var movement = move * (moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(_rb.position + movement);
        }
    }
}
