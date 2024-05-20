using Managers;
using UnityEngine;
using XLua;

namespace Entity
{
    [CSharpCallLua]
    public class Enemy : BaseEntity
    {
        protected override void Start()
        {
            base.Start();

            CurrentHealth = Health;
            GameManager.OnGameEvent += OnGameEvent;
        }

        private void OnDestroy()
        {
            GameManager.OnGameEvent -= OnGameEvent;
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

        protected override void OnDeath()
        {
            // Death logic
            Debug.Log($"{name} has died.");
            Destroy(gameObject);
        }
    }

    public static class LuaEnemies
    {
        public const string MeleeEnemy = "enemies/melee_enemy.lua";
    }
}
