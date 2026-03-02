using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using Entity.Pathfinding;
using Entity.Player;
using Managers;
using Scriptable;
using Systems.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity.Enemies
{
    public class Enemy : BaseEntity
    {
        [TitleHeader("Enemy Data")]
        [SerializeField] private EnemyStatsData statsData;

        protected Pathfinder Pathfinder;
        protected Transform Target;
        protected BaseEntity TargetEntity => Target ? Target.GetComponent<BaseEntity>() : null;
        protected List<Node> CurrentPath;
        protected int CurrentPathIndex;

        protected bool IsGameOver;

        #region Unity Events

        protected virtual void Start()
        {
            if (statsData)
                ApplyEnemyStats(statsData);

            Pathfinder = GetComponent<Pathfinder>();

            StartCoroutine(UpdatePath());

            GameManager.OnGameEvent += OnGameEvent;
            GameManager.OnGameOver += () => IsGameOver = true;
        }

        protected virtual void FixedUpdate()
        {
            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count || IsGameOver)
            {
                Animator.SetBool(IsMovingHash, false);

                if (!IsGameOver) return;
                StopAllCoroutines();

                return;
            }

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.WorldPosition;
            var distance = Vector2.Distance(transform.position, targetPosition);
            var canMove = distance > 0.1f;

            Animator.SetBool(IsMovingHash, canMove);

            if (canMove)
            {
                var direction = (targetPosition - (Vector2)transform.position).normalized;
                Rb.MovePosition(Rb.position + direction * (Speed * Time.fixedDeltaTime));

                var scale = transform.localScale;
                transform.localScale = scale.SetX(direction.x < 0 ? -1 : 1);
            }
            else
            {
                CurrentPathIndex++;
            }
        }

        protected virtual void LateUpdate()
        {
            // Continuously find the nearest player.
            Target = GetNearestPlayer()?.transform;
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

        #endregion

        protected void ApplyEnemyStats(EnemyStatsData data)
        {
            ApplyStats(data);
            this.GetOrCreateAttribute(Attribute.Points, data.points);
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
        }

        public override void OnDeath()
        {
            onDeathEvent?.Invoke();
            Rb.constraints = RigidbodyConstraints2D.FreezeAll;

            base.OnDeath();

            Collider.enabled = false;

            Animator.SetTrigger(IsDeadHash);
            StartCoroutine(DeathAnimation());
        }

        IEnumerator DeathAnimation()
        {
            // Wait a frame so the animator transitions into the death state
            yield return null;
            var animationDuration = Animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationDuration);
            OnDeathAnimation();
        }

        protected virtual void SpawnOrb()
        {
            if (GameManager.Instance.LocalMultiplayer || isServer)
            {
                var orb = PrefabManager.Create<Orb>(PrefabType.Orb);
                orb.transform.position = transform.position;
                orb.points = this.GetAttributeValue<long>(Attribute.Points);
            }
        }

        #region Player Detection

        /// <summary>
        /// Finds the nearest player.
        /// </summary>
        protected PlayerController GetNearestPlayer()
        {
            var players = EntityManager.Instance.GetPlayers();

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
            var players = EntityManager.Instance.GetPlayers();

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
            var players = EntityManager.Instance.GetPlayers()
                .Where(player => player).ToList();

            return players.Count == 0 ? null : players[Random.Range(0, players.Count)];
        }

        /// <summary>
        /// Finds the nearest moving player by velocity.
        /// </summary>
        protected PlayerController GetNearestMovingPlayer()
        {
            var players = EntityManager.Instance.GetPlayers();

            var movingPlayers = players
                .Where(player => player && player.IsMoving)
                .ToList();

            var closestMovingPlayer = movingPlayers
                .OrderBy(player => Vector2.Distance(transform.position, player.transform.position))
                .FirstOrDefault();

            return !closestMovingPlayer ? null : closestMovingPlayer;
        }

        /// <summary>
        /// Gets a random walkable position around the specified position within a given radius. Iterative.
        /// </summary>
        protected Vector2? GetRandomWalkablePositionAround(Vector2 center, float radius, int maxAttempts = 10)
        {
            var grid = Pathfinder.grid;

            for (int i = 0; i < maxAttempts; i++)
            {
                var randomPosition = center + Random.insideUnitCircle * radius;
                var node = grid!.GetNode(randomPosition);

                if (node != null && node.isWalkable)
                    return randomPosition;
            }

            Debug.LogWarning($"Could not find walkable position around {center} after {maxAttempts} attempts");
            return null;
        }
        #endregion
    }

}
