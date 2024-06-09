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
        [SerializeField] private float attackRange;
        [SerializeField] private float attackInterval;

        private float _attackTimer;

        protected override void Start()
        {
            base.Start();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (Target && Vector2.Distance(transform.position, Target.position) <= attackRange)
            {
                HandleAttack();
            }
        }

        // TODO: Only pathfind when the target is out of range.

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
                CmdSpawnProjectile(spawnPosition, spawnRotation);
            }
            else
            {
                // Decrease the attack timer.
                _attackTimer -= Time.deltaTime;
            }
        }

        [Command]
        private void CmdSpawnProjectile(Vector3 position, Quaternion rotation)
        {
            var projectile = PrefabManager.Create<ProjectileBase>(projectileType);
            projectile.owner = this;
            projectile.transform.SetPositionAndRotation(position, rotation);

            NetworkServer.Spawn(projectile.gameObject);
        }
    }
}
