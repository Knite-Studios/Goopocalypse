using Entity.Player;
using Managers;
using UnityEngine;

namespace Entity.StateMachines
{
    public class PlayerBaseState : BaseState<BaseEntity>
    {
        protected readonly PlayerController player;
        protected Vector2 Direction;

        protected static readonly int Moving = Animator.StringToHash("IsMoving");
        protected static readonly int Idle = Animator.StringToHash("IsIdle");

        public PlayerBaseState(string name, BaseEntity owner) : base(name, owner)
        {
            player = owner as PlayerController;
        }

        public override void UpdateState()
        {
            player.ChangeState(player.IsMoving
                ? player.MovingState
                : player.IdleState);
        }

        protected void HandleMovement()
        {
            var move = InputManager.Movement.ReadValue<Vector2>();
            if (move != Vector2.zero) Direction = move.normalized;

            // Flip the sprite based on the direction it's facing.
            var scale = player.transform.localScale;
            player.transform.localScale = scale.SetX(Direction.x < 0 ? -1 : 1);

            var movement = move * (player.Speed * Time.fixedDeltaTime);
            player.Rb.MovePosition(player.Rb.position + movement);
        }
    }
}
