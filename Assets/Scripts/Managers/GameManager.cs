using System;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static Action OnGameStart;
        public static UnityAction<GameEvent> OnGameEvent;

        protected override void OnAwake()
        {
            InputManager.Initialize();
            LuaManager.Initialize();
            WaveManager.Initialize();
            PrefabManager.Initialize();
        }
    }

    public struct GameEvent
    {
        public GameEventType Type;

        public Transform Target;
    }

    public enum GameEventType
    {
        ChestSpawned,
        EnemyKilled
    }
}
