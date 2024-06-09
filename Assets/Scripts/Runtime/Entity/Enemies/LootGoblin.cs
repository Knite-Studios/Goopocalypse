using UnityEngine;

namespace Entity.Enemies
{
    public class LootGoblin : Enemy
    {
        protected override void FixedUpdate()
        {
            if (Target)
            {
                var direction = (transform.position - Target.position).normalized;
                Rb.MovePosition(Rb.position + direction.ToVector2() * (Speed * Time.fixedDeltaTime));
            }

            // TODO: Bounce off the boundery walls logic.
        }
    }
}
