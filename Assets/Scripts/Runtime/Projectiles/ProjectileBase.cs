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
        
        private float _timer;

        private void OnEnable()
        {
            _timer = lifetime;
        }
        
        private void Update()
        {
            transform.position += transform.right * (speed * Time.deltaTime);
            
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                PrefabManager.ReturnToPool(prefabType, gameObject);
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer) return;
            
            if (other.TryGetComponent(out BaseEntity damageable))
            {
                // TODO:
                // - Apply damage to the entity.
                // - Filter enemies and heroes.
            }
            
            PrefabManager.ReturnToPool(prefabType, gameObject);
        }

        protected virtual void OnAnimationEnd()
        {
            Destroy(gameObject);
        }

        public void Reset()
        {
            _timer = lifetime;
        }
    }
}