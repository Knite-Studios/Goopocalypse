using Attributes;
using UnityEngine;

namespace Entity
{
    public class ArrowIndicator : MonoBehaviour
    {
        [TitleHeader("Arrow Indicator Settings")]
        [SerializeField] private float orbitDistance = 2.0f;
        [SerializeField] private float maxOpacityDistance = 20.0f;
        [SerializeField] private AnimationCurve opacityCurve;

        private Transform _owner;
        private Transform _target;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!_target || !_owner) return;

            var direction = (_target.position - _owner.position).normalized;
            var distance = Vector3.Distance(_target.position, _owner.position);

            transform.position = _owner.position + direction * orbitDistance;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 180);

            // Adjust the opacity of the arrow based on the distance from the target.
            var opacity = opacityCurve.Evaluate(distance / maxOpacityDistance);
            // Debug.Log($"<color=red>{opacity}</color>");
            var color = _spriteRenderer.color;
            color.a = opacity;
            _spriteRenderer.color = color;
        }

        public void SetTarget(Transform owner, Transform target)
        {
            _owner = owner;
            _target = target;
            gameObject.SetActive(true);
        }

        public void DisableArrow()
            => gameObject.SetActive(false);
    }
}
