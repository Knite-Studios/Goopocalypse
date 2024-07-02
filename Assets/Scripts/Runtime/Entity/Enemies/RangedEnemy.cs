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

        [TitleHeader("Range Enemy Audio Settings")]
        [SerializeField] private AudioClip shootSound;

        private float _attackTimer;

        private Vector3 _spawnPosition;
        private float _angle;
        private Quaternion _spawnRotation;

        protected override void Start()
        {
            base.Start();

            _attackTimer = attackInterval;
        }

        protected override void FixedUpdate()
        {
            if (IsGameOver)
            {
                StopAllCoroutines();
                return;
            }

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
            var isReadyToAttack = _attackTimer <= 0;

            // Remain in idle state if not ready to attack.
            Animator.SetBool(IsIdleHash, !isReadyToAttack);

            // Check if the attack timer is ready.
            if (_attackTimer <= 0)
            {
                // Set the animator to attacking.
                Animator.SetBool(IsAttackingHash, true);
                // Reset the attack timer.
                _attackTimer = attackInterval;

                // Calculate the spawn position.
                _spawnPosition = transform.position + direction;
                _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _spawnRotation = Quaternion.Euler(0, 0, _angle);
            }
            else
            {
                // Decrease the attack timer.
                _attackTimer -= Time.deltaTime;
            }
        }

        private void FollowTarget()
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

        /// <summary>
        /// Called from the animation event.
        /// </summary>
        public void OnAttackAnimation()
        {
            // Spawn the projectile.
            if (!GameManager.Instance.LocalMultiplayer)
                SpawnServerProjectile(_spawnPosition, _spawnRotation);
            else
                SpawnProjectile(_spawnPosition, _spawnRotation);
        }
    }
}
