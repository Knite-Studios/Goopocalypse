﻿using UnityEngine.InputSystem;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>
    {
        private Inputs _inputs;

        public static InputAction Movement => Instance._inputs.Player.Movement;
        /// <summary>
        /// This is used when the game is in local multiplayer mode.
        /// </summary>
        public static InputAction Movement2 => Instance._inputs.Player.Movement2;

        public static InputAction Attack => Instance._inputs.Player.Attack;
        public static InputAction Interact => Instance._inputs.Player.Interact;
        public static InputAction Dab => Instance._inputs.Player.Dab;
        public static InputAction Inventory => Instance._inputs.Player.Inventory;
        public static InputAction Menu => Instance._inputs.Player.Menu;
        public static InputAction Invite => Instance._inputs.Player.Invite;

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
