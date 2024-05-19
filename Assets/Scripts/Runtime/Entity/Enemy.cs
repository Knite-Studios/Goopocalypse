using Managers;
using UnityEngine;
using UnityEngine.AI;
using XLua;

namespace Entity
{
    [CSharpCallLua]
    public class Enemy : BaseEntity
    {
        /// <summary>
        /// Creates a new enemy instance.
        /// </summary>
        /// <param name="luaScript">The path to the enemy's Lua script.</param>
        public Enemy(string luaScript) : base(luaScript)
        {
            GameManager.OnGameEvent += OnGameEvent;
        }

        /// <summary>
        /// Handles game events.
        /// </summary>
        /// <param name="gameEvent">The game event.</param>
        private void OnGameEvent(GameEvent gameEvent)
        {
            switch (gameEvent.Type)
            {
                case GameEventType.ChestSpawned:
                    Debug.Log($"Moving to {gameEvent.Target.name}");
                    break;
            }
        }
    }

    public static class Enemies
    {
        public static Enemy MeleeEnemy => new("enemies/melee_enemy.lua");
    }
}
