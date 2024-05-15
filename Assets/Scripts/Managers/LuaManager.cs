using XLua;

namespace Managers
{
    public class LuaManager : Singleton<LuaManager>
    {
        public const string LuaRoot = "Scripts/Lua/";
        public const string SpecialAbilityFunc = "On_SpecialAbility";

        public static readonly LuaEnv luaEnv = new();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            luaEnv.Dispose();
        }
    }
}
