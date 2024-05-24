using System.Collections;
using System.Collections.Generic;
using Entity.Pathfinding;
using Managers;
using UnityEngine;
using XLua;

namespace Entity.Enemies
{
    [CSharpCallLua]
    public class Enemy : BaseEntity
    {
        protected Pathfinder Pathfinder;
        protected Transform Target;
        protected Rigidbody2D Rb;

        private List<Node> _currentPath;
        private int _currentPathIndex;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {
            InitializeEntityFromLua();

            CurrentHealth = MaxHealth;
            Pathfinder = GetComponent<Pathfinder>();

            StartCoroutine(FindTarget());
            StartCoroutine(UpdatePath());

            GameManager.OnGameEvent += OnGameEvent;
        }

        private void FixedUpdate()
        {
            if (_currentPath == null || _currentPathIndex >= _currentPath.Count) return;

            var node = _currentPath[_currentPathIndex];
            var targetPosition = node.worldPosition;

            if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                var direction = (targetPosition - (Vector2)transform.position).normalized;
                Rb.MovePosition(Rb.position + direction * (Speed * Time.fixedDeltaTime));
            }
            else
            {
                _currentPathIndex++;
            }
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

        protected virtual IEnumerator FindTarget()
        {
            while (!Target)
            {
                var player = GameObject.FindWithTag("Player"); // TODO: Change with entity manager later.
                if (player) Target = player.transform;
                yield return new WaitForSeconds(1.0f);
            }
        }

        protected virtual IEnumerator UpdatePath()
        {
            while (true)
            {
                if (Target)
                {
                    _currentPath = Pathfinder.FindPath(Target.position);
                    _currentPathIndex = 0;
                }

                yield return new WaitForSeconds(0.3f);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }

    public static class LuaEnemies
    {
        public const string MeleeEnemy = "enemies/melee_enemy.lua";
    }
}
