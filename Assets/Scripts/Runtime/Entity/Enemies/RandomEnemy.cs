using System.Collections;
using Attributes;
using UnityEngine;

namespace Entity.Enemies
{
    public class RandomEnemy : Enemy
    {
        [TitleHeader("Random Enemy Settings")]
        [SerializeField] private Vector2 randomPoint = new Vector2(-5.0f, 5.0f);
        [SerializeField] private float detectionDistance = 5.0f;

        protected override IEnumerator UpdatePath()
        {
            while (true)
            {
                if (Target && Vector2.Distance(transform.position, Target.position) <= detectionDistance)
                {
                    CurrentPath = Pathfinder.FindPath(Target.position);
                }
                else
                {
                    var randomPosition = new Vector2(Random.Range(randomPoint.x, randomPoint.y),
                        Random.Range(randomPoint.x, randomPoint.y));
                    CurrentPath = Pathfinder.FindPath(randomPosition);
                }

                CurrentPathIndex = 0;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
