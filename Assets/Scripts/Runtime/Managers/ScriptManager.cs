using XLua;

namespace Managers
{
    /// <summary>
    /// Handles the Lua scripting environment for entity stats.
    /// </summary>
    public class ScriptManager : MonoSingleton<ScriptManager>
    {
        public const string LuaRoot = "Lua/";

        public const string SpecialAbilityFunc = "On_SpecialAbility";
        public const string BehaviorStartFunc = "On_MonoStart";
        public const string BehaviorUpdateFunc = "On_MonoUpdate";

        /// <summary>
        /// Reference to the Lua script engine.
        /// </summary>
        public static LuaEnv Environment => Instance._luaEnv;

        private readonly LuaEnv _luaEnv = new();

        private void Start()
        {
            // TODO: Replace with Unity-native

            // Initialize Lua environment
            _luaEnv.AddLoader(CustomLuaLoader);

            InitializeNativeScripting();
        }

        private byte[] CustomLuaLoader(ref string filepath)
        {
            // Custom Lua loading logic here
            return null;
        }

        private void InitializeNativeScripting()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _luaEnv?.Dispose();
        }

        #region Manager References

        public WaveManager WaveManager => WaveManager.Instance;
        public GameManager GameManager => GameManager.Instance;
        public LobbyManager LobbyManager => LobbyManager.Instance;
        public SettingsManager SettingsManager => SettingsManager.Instance;
        public AudioManager AudioManager => AudioManager.Instance;

        #endregion
    }
}
