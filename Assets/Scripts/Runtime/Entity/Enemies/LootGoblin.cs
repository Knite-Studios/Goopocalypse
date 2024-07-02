using System.Collections;
using Attributes;
using Managers;
using UnityEngine;

namespace Entity.Enemies
{
    public class LootGoblin : Enemy
    {
        [TitleHeader("Loot Goblin Settings")]
        [SerializeField] private float patrolRadius = 5.0f;
        [SerializeField] private float detectionRadius = 5.0f;
        [SerializeField] private float patrolCooldown = 2.0f;

        private bool _isPatrolling = true;

        protected override IEnumerator UpdatePath()
        {
            while (true)
            {
                if (Target && Vector2.Distance(transform.position, Target.position) <= detectionRadius)
                {
                    // Run away from the player continuously.
                    while (Vector2.Distance(transform.position, Target.position) <= detectionRadius)
                    {
                        var fleeDirection = (transform.position - Target.position).normalized;
                        var fleePosition = (Vector2)transform.position + fleeDirection.ToVector2() * patrolRadius;

                        // Add randomness to the flee position.
                        fleePosition += Random.insideUnitCircle * (patrolRadius * 0.5f);

                        CurrentPath = Pathfinder.FindPath(fleePosition);
                        CurrentPathIndex = 0;
                        yield return new WaitForSeconds(0.3f);
                    }
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
                        if (randomPosition.HasValue)
                        {
                            CurrentPath = Pathfinder.FindPath(randomPosition.Value);
                            CurrentPathIndex = 0;
                        }
                    }

                    yield return new WaitForSeconds(patrolCooldown);
                }
            }

            // ReSharper disable once IteratorNeverReturns
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
