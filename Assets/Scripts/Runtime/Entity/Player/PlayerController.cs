using Entity.StateMachines;
using Managers;
using UnityEngine;

namespace Entity.Player
{
    public class PlayerController : Player
    {
        [HideInInspector] public IdleState IdleState;
        [HideInInspector] public MovingState MovingState;

        public bool IsMoving => InputManager.Movement.ReadValue<Vector2>() != Vector2.zero;

        private BaseState<BaseEntity> _currentState;

        protected override void Start()
        {
            if (!isLocalPlayer) return;

            base.Start();

            // Initialize the player states.
            InitializeStates();
            ChangeState(IdleState);
        }

        private void Update()
        {
            _currentState?.UpdateState();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            _currentState?.FixedUpdateState();
        }

        private void InitializeStates()
        {
            IdleState = new IdleState("Idle", this);
            MovingState = new MovingState("Moving", this);
        }

        public void ChangeState(PlayerBaseState state)
        {
            _currentState?.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }
    }
}
