using Entity.Player;
using Managers;
using UnityEngine;

namespace Entity.StateMachines
{
    public class PlayerBaseState : BaseState<BaseEntity>
    {
        protected readonly PlayerController player;
        protected Vector2 Direction;

        private readonly Camera _camera;
        private ArrowIndicator _arrowIndicator;

        public PlayerBaseState(string name, BaseEntity owner) : base(name, owner)
        {
            player = owner as PlayerController;
            _camera = Camera.main;
        }

        public override void UpdateState()
        {
            player.ChangeState(player.IsMoving
                ? player.MovingState
                : player.IdleState);

            HandleArrowIndicator();

            // Flip the sprite based on the direction it's facing.
            HandleSpriteFlip();
        }

        private void HandleSpriteFlip()
        {
            var scale = player.transform.localScale;
            if (Mathf.Abs(Direction.x) > 0.1f)
                player.transform.localScale = scale.SetX(Direction.x < 0 ? -1 : 1);
        }

        protected void HandleMovement()
        {
            var move = player.input.ReadValue<Vector2>();
            if (move != Vector2.zero) Direction = move.normalized;

            var movement = move * (player.Speed * Time.fixedDeltaTime);
            player.Rb.MovePosition(player.Rb.position + movement);

            // Simulate a slime movement effect by applying force in the direction of movement.
            player.Rb.AddForce(move * (0.5f * Time.fixedDeltaTime), ForceMode2D.Impulse);

            // Hacky way to ensure the player slides to the direction they're facing
            // even after abruptly switching directions. :)
            var velocity = player.Rb.velocity;
            switch (Direction.x)
            {
                case < 0 when velocity.x > 0:
                case > 0 when velocity.x < 0:
                    velocity.x = -velocity.x;
                    break;
            }

            switch (Direction.y)
            {
                case < 0 when velocity.y > 0:
                case > 0 when velocity.y < 0:
                    velocity.y = -velocity.y;
                    break;
            }

            player.Rb.velocity = new Vector2(velocity.x, velocity.y);
            player.Rb.drag = 1.0f;
        }

        private void HandleArrowIndicator()
        {
            // Ensure this only runs for the local player.
            if (!player.isLocalPlayer) return;

            // Find other the other player.
            var otherPlayer = EntityManager.Instance.players
                .Find(other => other != player);

            if (!otherPlayer) return;

            // Check if the other player is off-screen.
            var viewPortPosition = _camera.WorldToViewportPoint(otherPlayer.transform.position);
            var isOffScreen = viewPortPosition.x < 0 || viewPortPosition.x > 1 ||
                              viewPortPosition.y < 0 || viewPortPosition.y > 1;

            // Create and handle the arrow indicator.
            if (isOffScreen)
            {
                if (!_arrowIndicator)
                    _arrowIndicator = PrefabManager.Create<ArrowIndicator>(PrefabType.ArrowIndicator);

                _arrowIndicator?.SetTarget(player.transform, otherPlayer.transform);
            }
            else
            {
                _arrowIndicator?.DisableArrow();
            }
        }
    }
}
