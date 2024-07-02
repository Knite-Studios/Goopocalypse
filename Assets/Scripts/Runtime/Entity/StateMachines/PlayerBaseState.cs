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
        }

        protected void HandleMovement()
        {
            var move = player.input.ReadValue<Vector2>();
            if (move != Vector2.zero) Direction = move.normalized;

            // Flip the sprite based on the direction it's facing.
            var scale = player.transform.localScale;
            player.transform.localScale = scale.SetX(Direction.x < 0 ? -1 : 1);

            var movement = move * (player.Speed * Time.fixedDeltaTime);
            player.Rb.MovePosition(player.Rb.position + movement);
        }

        private void HandleArrowIndicator()
        {
            if (GameManager.Instance.LocalMultiplayer) return;

            var otherPlayer = EntityManager.Instance.players
                .Find(other => other != player);

            if (!otherPlayer) return;

            var viewPortPosition = _camera.WorldToViewportPoint(otherPlayer.transform.position);
            var isOffScreen = viewPortPosition.x < 0 || viewPortPosition.x > 1 ||
                              viewPortPosition.y < 0 || viewPortPosition.y > 1;

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
