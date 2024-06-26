using System.Collections;
using UnityEngine;

namespace Entity.Enemies
{
    public class DashEnemy : Enemy
    {
        [SerializeField] private float detectionRadius = 5.0f;
        [SerializeField] private float chargeTime = 1.0f;
        [SerializeField] private float dashSpeed = 10.0f;

        private bool _isCharging;
        private bool _isDashing;

        protected override void FixedUpdate()
        {
            if (!Target)
            {
                StopAllCoroutines();
                return;
            }

            if (_isDashing) return;

            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count) return;

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.WorldPosition;

            if (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                var direction = (targetPosition - (Vector2)transform.position).normalized;
                Rb.MovePosition(Rb.position + direction * (Speed * Time.fixedDeltaTime));
            }
            else
            {
                CurrentPathIndex++;
            }

            if (Target && Vector2.Distance(transform.position, Target.position) <= detectionRadius && !_isCharging)
                StartCoroutine(ChargeAndDash());
        }

        private IEnumerator ChargeAndDash()
        {
            _isCharging = true;

            // Stop moving during the charge.
            CurrentPath = null;

            // Wait for the charge time.
            yield return new WaitForSeconds(chargeTime);

            // Dash towards the player.
            _isCharging = false;
            _isDashing = true;
            var dashDirection = (Target.position - transform.position).normalized;
            var dashDuration = detectionRadius / dashSpeed;

            var elapsedTime = 0f;
            while (elapsedTime < dashDuration)
            {
                Rb.MovePosition(Rb.position + dashDirection.ToVector2() * (dashSpeed * Time.fixedDeltaTime));
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isDashing = false;
        }
    }
}
