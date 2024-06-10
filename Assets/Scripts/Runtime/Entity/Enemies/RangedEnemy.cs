using Attributes;
using Managers;
using Mirror;
using Projectiles;
using UnityEngine;

namespace Entity.Enemies
{
    public class RangedEnemy : Enemy
    {
        [TitleHeader("Ranged Enemy Settings")]
        [SerializeField] private PrefabType projectileType;
        [SerializeField] private float attackRange = 5.0f;
        [SerializeField] private float attackInterval = 1.5f;

        private float _attackTimer;

        protected override void Start()
        {
            base.Start();

            _attackTimer = attackInterval;
        }

        protected override void FixedUpdate()
        {
            if (!isServer) return;

            if (!Target) return;

            var distance = Vector2.Distance(transform.position, Target.transform.position);

            if (distance <= attackRange)
                HandleAttack();
            else
                FollowTarget();
        }

        private void HandleAttack()
        {
            // Get Target's direction.
            var direction = (Target.position - transform.position).normalized;

            // Check if the attack timer is ready.
            if (_attackTimer <= 0)
            {
                // Reset the attack timer.
                _attackTimer = attackInterval;

                // Calculate the spawn position.
                var spawnPosition = transform.position + direction;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var spawnRotation = Quaternion.Euler(0, 0, angle);

                // Spawn the projectile.
                SpawnProjectile(spawnPosition, spawnRotation);
            }
            else
            {
                // Decrease the attack timer.
                _attackTimer -= Time.deltaTime;
            }
        }

        private void FollowTarget()
        {
            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count) return;

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.worldPosition;

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

        [Server]
        private void SpawnProjectile(Vector3 position, Quaternion rotation)
        {
            var projectile = PrefabManager.Create<ProjectileBase>(projectileType);
            projectile.owner = this;
            projectile.transform.SetPositionAndRotation(position, rotation);

            NetworkServer.Spawn(projectile.gameObject);
        }
    }
}
