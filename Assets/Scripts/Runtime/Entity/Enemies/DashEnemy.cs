using System.Collections;
using Attributes;
using UnityEngine;

namespace Entity.Enemies
{
    public class DashEnemy : Enemy
    {
        [TitleHeader("Dash Enemy Settings")]
        [SerializeField] private float dashRange = 5.0f;
        [SerializeField] private float dashSpeed = 10.0f;

        private bool _isDashing;


        protected override void FixedUpdate()
        {
            if (!Target || IsGameOver)
            {
                StopAllCoroutines();
                return;
            }

            if (_isDashing) return;

            if (!Target) return;

            var distance = Vector2.Distance(transform.position, Target.transform.position);
            if (distance <= dashRange)
                HandleAttack();
            else
                FollowTarget();
        }

        private void HandleAttack()
        {
            // Stop moving during the charge.
            CurrentPath = null;

            // Start charging towards the player (shown by the animation).
            Animator.SetBool(IsAttackingHash, true);
        }

        private IEnumerator Dash()
        {
            // Dash towards the player.
            _isDashing = true;
            var dashDirection = (Target.position - transform.position).normalized;
            var dashDuration = dashRange / dashSpeed;

            var elapsedTime = 0f;
            while (elapsedTime < dashDuration)
            {
                Rb.MovePosition(Rb.position + dashDirection.ToVector2() * (dashSpeed * Time.fixedDeltaTime));
                elapsedTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isDashing = false;
            Animator.SetBool(IsAttackingHash, false);
        }

        private void FollowTarget()
        {
            if (CurrentPath == null || CurrentPathIndex >= CurrentPath.Count)
            {
                Animator.SetBool(IsMovingHash, false);
                return;
            }

            var node = CurrentPath[CurrentPathIndex];
            var targetPosition = node.WorldPosition;
            var distance = Vector2.Distance(transform.position, targetPosition);
            var canMove = distance > 0.1f;

            // If we can move then set the animator to moving and not idle.
            Animator.SetBool(IsMovingHash, canMove);
            Animator.SetBool(IsIdleHash, !canMove);

            if (canMove)
            {
                var direction = (targetPosition - (Vector2)transform.position).normalized;
                Rb.MovePosition(Rb.position + direction * (Speed * Time.fixedDeltaTime));
                Rb.AddForce(direction * Speed, ForceMode2D.Force);
            }
            else
            {
                CurrentPathIndex++;
            }
        }

        /// <summary>
        /// Called from the animation event.
        /// </summary>
        public void OnAttackAnimation()
        {
            StartCoroutine(Dash());
        }
    }
}
