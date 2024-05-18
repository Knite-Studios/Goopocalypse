using Entity;
using Interfaces;
using Managers;
using Mirror;
using Player;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileBase : NetworkBehaviour, IPoolObject
    {
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float speed = 10.0f;
        [SerializeField] protected float lifetime = 5.0f;
        [SerializeField] protected PrefabType prefabType;

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
                PrefabManager.ReturnToPool(prefabType, gameObject);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer) return;

            // Check if the other object is the owner.
            if (other.Has<PlayerController>())
            {
                return;
            }

            if (other.TryGetComponent(out BaseEntity damageable))
            {
                // TODO:
                // - Apply damage to the entity.
                // - Filter enemies and heroes.
            }
        }

        protected virtual void OnAnimationEnd()
        {
            PrefabManager.ReturnToPool(prefabType, gameObject);
        }
    }
}
