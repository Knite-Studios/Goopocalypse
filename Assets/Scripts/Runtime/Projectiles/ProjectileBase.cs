using Entity;
using Interfaces;
using Managers;
using Mirror;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileBase : NetworkBehaviour, IPoolObject
    {
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float speed = 10.0f;
        [SerializeField] protected float lifetime = 5.0f;
        [SerializeField] protected PrefabType prefabType;

        public BaseEntity owner;

        private float _timer;

        private void OnEnable()
        {
            Reset();
        }

        public void Reset()
        {
            _timer = lifetime;
        }

        private void Update()
        {
            var objTransform = transform;
            objTransform.position += objTransform.right * (speed * Time.deltaTime);

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                PrefabManager.Destroy(prefabType, gameObject);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out BaseEntity entity)) return;

            // Check if the projectile should do damage.
            if (entity == owner) return;
            if (entity.GetType() == owner.GetType()) return;
            if (entity.GetType().BaseType == owner.GetType().BaseType) return;

            // Apply damage.
            if (isServer)
                entity.Damage(entity.MaxHealth, true);
            else
                entity.OnDeath();

        }

        protected virtual void OnAnimationEnd()
        {
            PrefabManager.Destroy(prefabType, gameObject);
        }
    }
}
