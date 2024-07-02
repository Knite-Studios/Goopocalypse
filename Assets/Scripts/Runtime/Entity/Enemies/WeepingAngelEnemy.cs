using System.Collections;
using Managers;
using UnityEngine;

namespace Entity.Enemies
{
    public class WeepingAngelEnemy : Enemy
    {
        protected override void LateUpdate()
        {
            Target = !GetNearestMovingPlayer() ? null : GetNearestMovingPlayer().transform;
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

        /// <summary>
        /// Method called for death animations.
        /// </summary>
        public override void OnDeathAnimation()
        {
            base.OnDeathAnimation();
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
