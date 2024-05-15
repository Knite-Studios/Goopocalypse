namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        protected override void OnAwake()
        {
            InputManager.Initialize();
            LuaManager.Initialize();
        }
    }
}
