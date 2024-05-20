using UnityEngine.InputSystem;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>
    {
        private Inputs _inputs;

        public static InputAction Movement => Instance._inputs.Player.Movement;
        public static InputAction Attack => Instance._inputs.Player.Attack;
        public static InputAction Mouse => Instance._inputs.Player.Mouse;
        public static InputAction Interact => Instance._inputs.Player.Interact;
        public static InputAction Dab => Instance._inputs.Player.Dab;
        public static InputAction Inventory => Instance._inputs.Player.Inventory;
        public static InputAction Menu => Instance._inputs.Player.Menu;


        protected override void OnAwake()
        {
            _inputs = new Inputs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputs.Enable();
        }

        private void OnDisable()
        {
            _inputs.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _inputs.Dispose();
        }
    }
}
