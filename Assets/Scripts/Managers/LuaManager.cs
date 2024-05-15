using System.IO;
using UnityEngine;
using XLua;

namespace Managers
{
    public class LuaManager : Singleton<LuaManager>
    {
        private const string LuaRoot = "Lua/";
        public static readonly LuaEnv luaEnv = new LuaEnv();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            luaEnv.Dispose();
        }

        /// <summary>
        /// An alternative for lua_doString. This method will look for the lua script located in the StreamingAssets folder.
        /// </summary>
        /// <param name="scriptName">The name of the lua script to execute.</param>
        public void DoFile(string scriptName)
        {
            var path = Path.Combine(Application.streamingAssetsPath, LuaRoot + scriptName + ".lua");
            luaEnv.DoString(File.ReadAllText(path));
        }
    }
}