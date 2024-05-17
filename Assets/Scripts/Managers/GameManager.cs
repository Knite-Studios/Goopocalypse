using System;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action OnGameStart;

        protected override void OnAwake()
        {
            InputManager.Initialize();
            LuaManager.Initialize();
            WaveManager.Initialize();
        }
    }
}
