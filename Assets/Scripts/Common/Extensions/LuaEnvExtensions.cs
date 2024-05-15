using System.IO;
using Managers;
using UnityEngine;
using XLua;

namespace Common.Extensions
{
    public static class LuaEnvExtensions
    {
        /// <summary>
        /// Load the Lua script into the environment from 'StreamingAssets'.
        /// </summary>
        /// <param name="env">The Lua environment.</param>
        /// <param name="filePath">The path (from StreamingAssets).</param>
        public static void DoFile(this LuaEnv env, string filePath)
        {
            if (!filePath.EndsWith(".lua"))
            {
                filePath += ".lua";
            }

            var path = Path.Combine(
                Application.streamingAssetsPath, LuaManager.LuaRoot, filePath);
            env.DoString(File.ReadAllText(path));
        }
    }
}
