using Entity.StateMachines;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    public class PlayerController : Player
    {
        [HideInInspector] public IdleState IdleState;
        [HideInInspector] public MovingState MovingState;

        /// <summary>
        /// This is configurable by the game.
        /// </summary>
        public InputAction input;

        public bool IsMoving => input.ReadValue<Vector2>() != Vector2.zero;

        private BaseState<BaseEntity> _currentState;

        protected override void Awake()
        {
            input = InputManager.Movement;

            base.Awake();
        }

        protected override void Start()
        {
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
            if (!isLocalPlayer &&
                !GameManager.Instance.LocalMultiplayer) return;

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
