using Managers;
using UnityEngine;

namespace Entity.Player
{
    public class PlayerController : Player
    {
        public bool IsMoving { get; private set; }

        private Vector2 _direction;
        private static readonly int Moving = Animator.StringToHash("IsMoving");

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            HandleMovement();
        }

        private void HandleMovement()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            IsMoving = move != Vector2.zero;
            Animator.SetBool(Moving, IsMoving);
            if (move != Vector2.zero) _direction = move.normalized;

            // Flip the sprite based on the direction it's facing.
            var scale = transform.localScale;
            transform.localScale = scale.SetX(_direction.x < 0 ? -1 : 1);


            var movement = move * (Speed * Time.fixedDeltaTime);
            Rb.MovePosition(Rb.position + movement);
        }
    }
}
