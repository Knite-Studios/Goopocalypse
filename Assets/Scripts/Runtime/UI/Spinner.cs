using System;
using Common.Extensions;
using Managers;
using UnityEngine;

namespace Runtime.UI
{
    public class Spinner : MonoBehaviour
    {
        private delegate void LuaSpinnerUpdate(Transform transform);
        private LuaSpinnerUpdate _luaSpinnerUpdate;

        private void Start()
        {
            var env = LuaManager.Environment;
            env.DoFile("spinner");
            _luaSpinnerUpdate = env.Global.Get<LuaSpinnerUpdate>("On_Update");
        }

        private void Update()
        {
            _luaSpinnerUpdate?.Invoke(transform);
        }
    }
}
