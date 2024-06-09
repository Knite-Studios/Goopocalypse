using System.Collections;
using UnityEngine;

namespace Entity.Enemies
{
    public class DashEnemy : Enemy
    {
        [SerializeField] private float dashSpeed = 10.0f;
        [SerializeField] private float dashDistance = 5.0f;
        [SerializeField] private float dashDelay = 1.0f;

        private bool _isDashing;

        protected override void FixedUpdate()
        {
            if (!_isDashing && Target && Vector2.Distance(transform.position, Target.position) <= dashDistance)
            {
                StartCoroutine(Dash());
            }

            base.FixedUpdate();
        }

        private IEnumerator Dash()
        {
            _isDashing = true;

            var direction = (Target.position - transform.position).normalized;
            var targetPosition = (Vector2)transform.position + direction.ToVector2() * dashDistance;
            var originalPosition = transform.position;
            var elapsedTime = 0.0f;

            while (elapsedTime < dashDelay)
            {
                elapsedTime += Time.deltaTime;
                Rb.MovePosition(Vector2.Lerp(originalPosition, targetPosition, elapsedTime / dashDelay));
                yield return null;
            }

            _isDashing = false;
        }
    }
}
