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
        [SerializeField] public Animator animator;

        [TitleHeader("Range Enemy Audio Settings")]
        [SerializeField] private AudioClip shootSound;

        private float _attackTimer;

        protected override void Start()
        {
            base.Start();

            _attackTimer = attackInterval;
        }

        protected override void FixedUpdate()
        {
            if (!Target) return;

            var distance = Vector2.Distance(transform.position, Target.transform.position);

            if (distance <= attackRange)
            {
                Animator.SetTrigger("IsAttacking");
                HandleAttack();
            }
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
                if (!GameManager.Instance.LocalMultiplayer)
                    SpawnServerProjectile(spawnPosition, spawnRotation);
                else
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
            var targetPosition = node.WorldPosition;

            if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                var direction = (targetPosition - (Vector2)transform.position).normalized;
                Rb.MovePosition(Rb.position + direction * (Speed * Time.fixedDeltaTime));
                Rb.AddForce(direction * Speed, ForceMode2D.Force);
            }
            else
            {
                CurrentPathIndex++;
            }
        }

        [Server]
        private void SpawnServerProjectile(Vector3 position, Quaternion rotation)
        {
            SpawnProjectile(position, rotation, true);
        }

        private void SpawnProjectile(Vector3 position, Quaternion rotation, bool server = false)
        {
            if (AudioSource.isPlaying) AudioSource.Stop();
            if (shootSound) AudioSource.PlayOneShot(shootSound);

            var projectile = PrefabManager.Create<ProjectileBase>(projectileType);
            projectile.owner = this;
            projectile.transform.SetPositionAndRotation(position, rotation);

            if (server) NetworkServer.Spawn(projectile.gameObject);
        }
    }
}
