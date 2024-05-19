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
}
