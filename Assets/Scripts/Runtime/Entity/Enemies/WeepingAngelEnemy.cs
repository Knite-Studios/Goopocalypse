using System.Collections;
using UnityEngine;

namespace Entity.Enemies
{
    public class WeepingAngelEnemy : Enemy
    {
        protected override void LateUpdate()
        {
            Target = GetNearestMovingPlayer() == null ? null : GetNearestMovingPlayer().transform;
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
