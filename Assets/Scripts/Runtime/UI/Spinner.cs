using Common.Extensions;
using Managers;
using UnityEngine;

namespace UI
{
    public class Spinner : MonoBehaviour
    {
        private delegate void LuaSpinnerUpdate(Transform transform);
        private LuaSpinnerUpdate _luaSpinnerUpdate;

        private void Start()
        {
            var env = ScriptManager.Environment;
            env.DoFile("spinner");
            _luaSpinnerUpdate = env.Global.Get<LuaSpinnerUpdate>(ScriptManager.BehaviorUpdateFunc);
        }

        private void Update()
        {
            _luaSpinnerUpdate?.Invoke(transform);
        }

        private void OnDisable() => _luaSpinnerUpdate = null;
    }
}
