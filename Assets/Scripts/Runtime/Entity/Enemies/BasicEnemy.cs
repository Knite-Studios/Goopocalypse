using Managers;

namespace Entity.Enemies
{
    public class BasicEnemy : Enemy
    {
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
