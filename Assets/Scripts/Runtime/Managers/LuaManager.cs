using XLua;

namespace Managers
{
    public class LuaManager : MonoSingleton<LuaManager>
    {
        public const string LuaRoot = "Scripts/Lua/";
        public const string SpecialAbilityFunc = "On_SpecialAbility";

        private readonly LuaEnv _luaEnv = new();

        public static LuaEnv Environment => Instance._luaEnv;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _luaEnv.Dispose();
        }
    }
}
