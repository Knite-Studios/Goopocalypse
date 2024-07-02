using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entity.Pathfinding;
using Entity.Player;
using Managers;
using Systems.Attributes;
using UnityEngine;
using XLua;
using Random = UnityEngine.Random;

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

        protected override void ApplyBaseStats(LuaTable stats)
        {
            base.ApplyBaseStats(stats);

            this.GetOrCreateAttribute(Attribute.Points, stats.Get<long>("points"));
        }

        protected virtual void FixedUpdate()
        {
            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count)
            {
                Animator.SetBool(IsMovingHash, false);
                return;
            }

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.WorldPosition;
            var distance = Vector2.Distance(transform.position, targetPosition);
            var canMove = distance > 0.1f;

            // If we can move then set the animator to moving and not idle.
            Animator.SetBool(IsMovingHash, canMove);
            Animator.SetBool(IsIdleHash, !canMove);

            if (canMove)
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

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.IsPlayer()) return;
            if (!other.gameObject.TryGetComponent(out BaseEntity entity)) return;

            if (GameManager.Instance.LocalMultiplayer)
                entity.OnDeath();
            else
                entity.Damage(entity.MaxHealth, true);
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

        public override void OnDeath()
        {
            onDeathEvent?.Invoke();
            Rb.constraints = RigidbodyConstraints2D.FreezeAll;

            base.OnDeath();

            EntityManager.UnregisterEnemy(this);

            var orb = PrefabManager.Create<Orb>(PrefabType.Orb);
            orb.transform.position = transform.position;
            orb.points = this.GetAttributeValue<long>(Attribute.Points);

            // TODO: Remove this line if we have enemy death animation.
            OnDeathAnimation();
        }

        /// <summary>
        /// Finds the nearest player.
        /// </summary>
        protected PlayerController GetNearestPlayer()
        {
            var players = EntityManager.Instance.players;

            var nearestPlayer = players
                .Where(player => player)
                .OrderBy(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return !nearestPlayer ? null : nearestPlayer;
        }

        /// <summary>
        /// Finds the furthest player.
        /// </summary>
        protected PlayerController GetFurthestPlayer()
        {
            var players = EntityManager.Instance.players;

            var furthestPlayer = players
                .Where(player => player)
                .OrderByDescending(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return !furthestPlayer ? null : furthestPlayer;
        }

        /// <summary>
        /// Finds a random player.
        /// </summary>
        protected PlayerController GetRandomPlayer()
        {
            var players = EntityManager.Instance.players
                .Where(player => player).ToList();

            return players.Count == 0 ? null : players[Random.Range(0, players.Count)];
        }

        /// <summary>
        /// Finds the nearest moving player by velocity.
        /// </summary>
        protected PlayerController GetNearestMovingPlayer()
        {
            var players = EntityManager.Instance.players;

            var movingPlayers = players
                .Where(player => player && player.IsMoving)
                .ToList();

            var closestMovingPlayer = movingPlayers
                .OrderBy(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return !closestMovingPlayer ? null : closestMovingPlayer;
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
