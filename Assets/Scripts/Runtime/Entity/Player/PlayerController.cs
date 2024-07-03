using Entity.StateMachines;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Player
{
    public class PlayerController : Player
    {
        internal IdleState IdleState;
        internal MovingState MovingState;

        /// <summary>
        /// This is configurable by the game.
        /// </summary>
        public InputAction input;

        public bool IsMoving => input.ReadValue<Vector2>() != Vector2.zero;

        private BaseState<BaseEntity> _currentState;
        private bool _isPaused;

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

            InputManager.Menu.canceled += HandlePause;
            GameManager.OnGameResume += HandleResume;
        }

        private void OnDestroy()
        {
            InputManager.Menu.canceled -= HandlePause;
            GameManager.OnGameResume -= HandleResume;
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

        private void HandlePause(InputAction.CallbackContext context)
        {
            _isPaused = !_isPaused;
            GameManager.OnGamePause?.Invoke(_isPaused);
        }

        private void HandleResume()
            => _isPaused = false;
    }
}
