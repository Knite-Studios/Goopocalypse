using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity.Pathfinding;
using Entity.Player;
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
        protected List<Node> CurrentPath;
        protected int CurrentPathIndex;

        protected virtual void Start()
        {
            InitializeEntityFromLua();

            Pathfinder = GetComponent<Pathfinder>();

            StartCoroutine(FindTarget());
            StartCoroutine(UpdatePath());

            GameManager.OnGameEvent += OnGameEvent;
        }

        protected virtual void FixedUpdate()
        {
            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count) return;

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.WorldPosition;

            if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                var direction = (targetPosition - (Vector2)transform.position).normalized;
                Rb.MovePosition(Rb.position + direction * (Speed * Time.fixedDeltaTime));
            }
            else
            {
                CurrentPathIndex++;
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
                var player = GetNearestPlayer();
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
                    CurrentPath = Pathfinder.FindPath(Target.position);
                    CurrentPathIndex = 0;
                }

                yield return new WaitForSeconds(0.3f);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Finds the nearest player.
        /// </summary>
        protected PlayerController GetNearestPlayer()
        {
            var players = EntityManager.Instance.players;
            // TODO: Temporary. REMOVE LATER.
            if (players.Count == 0) players = GameObject.FindGameObjectsWithTag("Player").ToList().ConvertAll(player
                => player.GetComponent<PlayerController>());

            var nearestPlayer = players
                .OrderBy(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return nearestPlayer;
        }

        /// <summary>
        /// Finds the furthest player.
        /// </summary>
        protected PlayerController GetFurthestPlayer()
        {
            var players = EntityManager.Instance.players;
            // TODO: Temporary. REMOVE LATER.
            if (players.Count == 0) players = GameObject.FindGameObjectsWithTag("Player").ToList().ConvertAll(player
                => player.GetComponent<PlayerController>());

            var furthestPlayer = players
                .OrderByDescending(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return furthestPlayer;
        }

        /// <summary>
        /// Finds a random player.
        /// </summary>
        protected PlayerController GetRandomPlayer()
        {
            var players = EntityManager.Instance.players;
            // TODO: Temporary. REMOVE LATER.
            if (players.Count == 0) players = GameObject.FindGameObjectsWithTag("Player").ToList().ConvertAll(player
                => player.GetComponent<PlayerController>());

            return players[Random.Range(0, players.Count)];
        }

        /// <summary>
        /// Finds the nearest moving player by velocity.
        /// </summary>
        protected PlayerController GetNearestMovingPlayer()
        {
            var players = EntityManager.Instance.players;
            // TODO: Temporary. REMOVE LATER.
            if (players.Count == 0)
                players = GameObject.FindGameObjectsWithTag("Player")
                    .Select(player => player.GetComponent<PlayerController>())
                    .ToList();

            var movingPlayers = players
                .Where(player => player.IsMoving)
                .ToList();

            var closestMovingPlayer = movingPlayers
                .OrderBy(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return closestMovingPlayer;
        }

        /// <summary>
        /// Gets a random walkable position around the specified position within a given radius.
        /// </summary>
        protected Vector2? GetRandomWalkablePositionAround(Vector2 center, float radius)
        {
            // NOTE: Might be best to do recurssion here.
            var grid = Pathfinder.grid;
            var randomPosition = center + Random.insideUnitCircle * radius;
            var node = grid!.GetNode(randomPosition);

            return node != null && node.isWalkable ? randomPosition : null;
        }
    }

    public static class LuaEnemies
    {
        public const string MeleeEnemy = "enemies/melee_enemy.lua";
    }
}
