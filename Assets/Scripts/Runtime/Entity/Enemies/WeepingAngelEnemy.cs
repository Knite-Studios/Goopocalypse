using System.Collections;
using UnityEngine;

namespace Entity.Enemies
{
    public class WeepingAngelEnemy : Enemy
    {
        protected override IEnumerator FindTarget()
        {
            while (!Target)
            {
                var player = GetNearestMovingPlayer();
                if (player) Target = player.transform;
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
