﻿using System.IO;
using Managers;
using UnityEngine;
using XLua;

namespace Common.Extensions
{
    public static class LuaEnvExtensions
    {
        /// <summary>
        /// Load the Lua script into the environment from 'Assets/Scripts/Lua'.
        /// </summary>
        /// <param name="env">The Lua environment.</param>
        /// <param name="filePath">The path (from Assets/Scripts/Lua).</param>
        public static void DoFile(this LuaEnv env, string filePath)
        {
            if (!filePath.EndsWith(".lua"))
            {
                filePath += ".lua";
            }

#if UNITY_EDITOR
            var path = Path.Combine(
                Application.streamingAssetsPath, ScriptManager.LuaRoot, filePath);
            env.DoString(File.ReadAllText(path));
#else
            var textAsset = Resources.Load<TextAsset>($"{ScriptManager.LuaRoot}{filePath}");
            env.DoString(textAsset.text);
#endif
        }
    }
}
