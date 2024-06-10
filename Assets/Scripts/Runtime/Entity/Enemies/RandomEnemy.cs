using System.Collections;
using Attributes;
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
    }
}
