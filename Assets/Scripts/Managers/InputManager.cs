using UnityEngine.InputSystem;

namespace Managers
{
    public class InputManager : Singleton<InputManager>
    {
        private Inputs _inputs;
        
        public static InputAction Movement => Instance._inputs.Player.Movement;
        public static InputAction Attack => Instance._inputs.Player.Attack;
        public static InputAction Mouse => Instance._inputs.Player.Mouse;

        protected override void OnAwake()
        {
            _inputs = new Inputs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputs.Enable();
        }

        private void OnDisable() => _inputs.Disable();
    }
}