using System.Collections;
using Attributes;
using Managers;
using UnityEngine;

namespace Entity.Enemies
{
    public class RandomEnemy : Enemy
    {
        [TitleHeader("Random Enemy Settings")]
        [SerializeField] private float patrolRadius = 5.0f;
        [SerializeField] private float detectionDistance = 5.0f;
        [SerializeField] private float patrolCooldown = 2.0f;

        private bool _isPatrolling = true;

        protected override void FixedUpdate()
        {
            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count || IsGameOver)
            {
                if (!IsGameOver) return;
                StopAllCoroutines();

                return;
            }

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.WorldPosition;
            var distance = Vector2.Distance(transform.position, targetPosition);
            var canMove = distance > 0.1f;

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

        protected override IEnumerator UpdatePath()
        {
            while (true)
            {
                if (Target && Vector2.Distance(transform.position, Target.position) <= detectionDistance)
                {
                    CurrentPath = Pathfinder.FindPath(Target.position);
                    _isPatrolling = false;
                }
                else
                {
                    if (!_isPatrolling)
                    {
                        _isPatrolling = true;
                    }
                    else
                    {
                        var randomPosition = GetRandomWalkablePositionAround(transform.position, patrolRadius);
                        if (randomPosition.HasValue) CurrentPath = Pathfinder.FindPath(randomPosition.Value);
                    }

                    yield return new WaitForSeconds(patrolCooldown);
                }

                CurrentPathIndex = 0;
                yield return new WaitForSeconds(0.3f);
            }
        }

        /// <summary>
        /// Method called for death animations.
        /// </summary>
        public override void OnDeathAnimation()
        {
            SpawnOrb();
            Dispose();
        }

        /// <summary>
        /// Method called for death sounds.
        /// </summary>
        public override void OnDeathSound()
        {
            if (AudioSource.isPlaying) AudioSource.Stop();
            AudioManager.Instance.PlayOneShot(deathSound, transform.position);
        }
    }
}
