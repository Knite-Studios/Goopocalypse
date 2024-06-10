using System.Collections;
using UnityEngine;

namespace Entity.Enemies
{
    public class WeepingAngelEnemy : Enemy
    {
        protected override IEnumerator FindTarget()
        {
            while (true)
            {
                var player = GetNearestMovingPlayer();
                Target = player ? player.transform : null;
                yield return new WaitForSeconds(0.3f);
            }
        }

        protected override IEnumerator UpdatePath()
        {
            while (true)
            {
                if (Target)
                {
                    CurrentPath = Pathfinder.FindPath(Target.position);
                    CurrentPathIndex = 0;
                }
                else
                {
                    CurrentPath = null;
                }

                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
