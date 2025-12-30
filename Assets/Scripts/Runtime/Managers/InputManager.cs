using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Utils;

namespace Managers
{
    public class InputManager : MonoSingleton<InputManager>
    {
        private Inputs _inputs;
        private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

        public static InputAction Movement => Instance?._inputs.Player.Movement;
        /// <summary>
        /// This is used when the game is in local multiplayer mode.
        /// </summary>
        public static InputAction Movement2 => Instance?._inputs.Player.Movement2;

        public static InputAction Attack => Instance?._inputs.Player.Attack;
        public static InputAction Interact => Instance?._inputs.Player.Interact;
        public static InputAction Dab => Instance?._inputs.Player.Dab;
        public static InputAction Inventory => Instance?._inputs.Player.Inventory;
        public static InputAction Menu => Instance?._inputs.Player.Menu;
        public static InputAction Invite => Instance?._inputs.Player.Invite;

        /// <summary>
        /// True when a rebinding operation is in progress.
        /// </summary>
        public static bool IsRebinding => Instance?._rebindOperation != null;

        /// <summary>
        /// Fired when a rebinding operation completes successfully.
        /// </summary>
        public static event Action<InputAction, int> OnRebindComplete;

        /// <summary>
        /// Fired when a rebinding operation is canceled.
        /// </summary>
        public static event Action<InputAction> OnRebindCanceled;

        /// <summary>
        /// Gets all rebindable actions (excludes composite Movement actions).
        /// </summary>
        public static IReadOnlyList<InputAction> RebindableActions => new[]
        {
            Attack, Interact, Dab, Inventory, Menu, Invite
        };

        protected override void OnAwake()
        {
            _inputs = new Inputs();
            LoadBindingOverrides();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputs?.Enable();
        }

        private void OnDisable()
        {
            _inputs?.Disable();
        }

        protected override void OnDestroy()
        {
            CancelRebind();

            if (_inputs != null)
            {
                _inputs.Disable();
                _inputs.Dispose();
                _inputs = null;
            }

            base.OnDestroy();
        }

        /// <summary>
        /// Loads saved binding overrides from PlayerPrefs.
        /// </summary>
        private void LoadBindingOverrides()
        {
            var overrides = PlayerPrefsUtil.KeybindOverrides;
            if (!string.IsNullOrEmpty(overrides))
            {
                _inputs.asset.LoadBindingOverridesFromJson(overrides);
            }
        }

        /// <summary>
        /// Saves current binding overrides to PlayerPrefs.
        /// </summary>
        public static void SaveBindingOverrides()
        {
            if (Instance?._inputs == null) return;
            var json = Instance._inputs.asset.SaveBindingOverridesAsJson();
            PlayerPrefsUtil.KeybindOverrides = json;
        }

        /// <summary>
        /// Resets all bindings to defaults and clears saved overrides.
        /// </summary>
        public static void ResetAllBindings()
        {
            if (Instance?._inputs == null) return;

            foreach (var map in Instance._inputs.asset.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }

            PlayerPrefsUtil.ClearKeybindOverrides();
        }

        /// <summary>
        /// Resets a specific action's bindings to defaults.
        /// </summary>
        public static void ResetBinding(InputAction action)
        {
            if (action == null) return;
            action.RemoveAllBindingOverrides();
            SaveBindingOverrides();
        }

        /// <summary>
        /// Starts an interactive rebinding operation for the specified action.
        /// </summary>
        /// <param name="action">The action to rebind.</param>
        /// <param name="bindingIndex">The binding index to rebind (-1 for first non-composite).</param>
        /// <param name="controlScheme">Optional control scheme filter (e.g., "Keyboard").</param>
        public static void StartRebind(InputAction action, int bindingIndex = -1, string controlScheme = null)
        {
            if (Instance == null || action == null) return;
            if (IsRebinding) CancelRebind();

            // Find the correct binding index if not specified
            if (bindingIndex < 0)
            {
                bindingIndex = FindBindingIndex(action, controlScheme);
                if (bindingIndex < 0) return;
            }

            action.Disable();

            Instance._rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(op => OnRebindOperationComplete(action, bindingIndex))
                .OnCancel(op => OnRebindOperationCanceled(action))
                .Start();
        }

        /// <summary>
        /// Cancels the current rebinding operation if one is in progress.
        /// </summary>
        public static void CancelRebind()
        {
            if (Instance?._rebindOperation == null) return;

            Instance._rebindOperation.Cancel();
            Instance._rebindOperation.Dispose();
            Instance._rebindOperation = null;
        }

        private static int FindBindingIndex(InputAction action, string controlScheme)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];
                if (binding.isComposite) continue;
                if (binding.isPartOfComposite) continue;

                if (string.IsNullOrEmpty(controlScheme) ||
                    binding.groups.Contains(controlScheme))
                {
                    return i;
                }
            }
            return -1;
        }

        private static void OnRebindOperationComplete(InputAction action, int bindingIndex)
        {
            Instance._rebindOperation?.Dispose();
            Instance._rebindOperation = null;

            action.Enable();
            SaveBindingOverrides();
            OnRebindComplete?.Invoke(action, bindingIndex);
        }

        private static void OnRebindOperationCanceled(InputAction action)
        {
            Instance._rebindOperation?.Dispose();
            Instance._rebindOperation = null;

            action.Enable();
            OnRebindCanceled?.Invoke(action);
        }

        /// <summary>
        /// Gets the display string for an action's binding.
        /// </summary>
        public static string GetBindingDisplayString(InputAction action, string controlScheme = "Keyboard")
        {
            if (action == null) return string.Empty;

            var bindingIndex = FindBindingIndex(action, controlScheme);
            if (bindingIndex < 0) return string.Empty;

            return action.GetBindingDisplayString(bindingIndex);
        }

        /// <summary>
        /// Checks if a binding conflicts with another action's binding.
        /// </summary>
        public static bool HasBindingConflict(InputAction action, int bindingIndex, out InputAction conflictingAction)
        {
            conflictingAction = null;
            if (Instance?._inputs == null || action == null) return false;

            var bindingPath = action.bindings[bindingIndex].effectivePath;
            if (string.IsNullOrEmpty(bindingPath)) return false;

            foreach (var otherAction in Instance._inputs.asset)
            {
                if (otherAction == action) continue;

                for (int i = 0; i < otherAction.bindings.Count; i++)
                {
                    if (otherAction.bindings[i].effectivePath == bindingPath)
                    {
                        conflictingAction = otherAction;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
