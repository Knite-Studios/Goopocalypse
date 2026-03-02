using System;
using System.Collections.Generic;
using Attributes;
using Managers;
using Mirror;
using Scriptable;
using Systems.Attributes;
using GameAttribute = Systems.Attributes.Attribute;
using UnityEngine;
using UnityEngine.Events;

namespace Entity
{
    /// <summary>
    /// The base class for all entities in the game.
    /// </summary>
    public abstract class BaseEntity : NetworkBehaviour, IAttributable, IDamageable, IDisposable
    {
        protected internal readonly int IsMovingHash = Animator.StringToHash("IsMoving");
        protected internal readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
        protected internal readonly int IsDeadHash = Animator.StringToHash("IsDead");

        [TitleHeader("Base Entity Settings")]
        public UnityEvent onDeathEvent;

        [TitleHeader("Audio Settings")]
        [SerializeField] protected AudioClip deathSound;

        public Dictionary<GameAttribute, object> Attributes { get; } = new();

        [field: SerializeField, ReadOnly]
        public int CurrentHealth { get; protected set; }
        public bool IsPlayer { get; protected set; }

        #region Attribute Getters

        public int MaxHealth => this.GetAttributeValue<int>(GameAttribute.MaxHealth);
        public float Speed => this.GetAttributeValue<float>(GameAttribute.Speed);
        public int Armor => this.GetAttributeValue<int>(GameAttribute.Armor);

        #endregion

        protected internal SpriteRenderer SpriteRenderer;
        protected internal Rigidbody2D Rb;
        protected internal Animator Animator;
        protected AudioSource AudioSource;
        protected internal Collider2D Collider;

        protected virtual void Awake()
        {
            if (netIdentity) EntityManager.RegisterEntity(this);

            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rb = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            AudioSource = GetComponent<AudioSource>();
            Collider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Applies base stats from a ScriptableObject into the Attribute system.
        /// </summary>
        protected virtual void ApplyStats(EntityStatsData stats)
        {
            this.GetOrCreateAttribute(GameAttribute.MaxHealth, stats.maxHealth);
            this.GetOrCreateAttribute(GameAttribute.Speed, stats.speed);
            this.GetOrCreateAttribute(GameAttribute.Armor, stats.armor);

            CurrentHealth = MaxHealth;
        }

        #region IDamageable

        [Server]
        public void Damage(int damage, bool trueDamage = false)
        {
            var finalDamage = trueDamage ? damage : Mathf.Max(0, damage - Armor);

            CurrentHealth -= finalDamage;
            OnHealthChange(-finalDamage);

            if (CurrentHealth > 0) return;

            CurrentHealth = 0;
            OnDeath();
        }

        [Server]
        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
            OnHealthChange(amount);
        }

        [ClientRpc]
        public virtual void OnHealthChange(int amount)
        {
            Debug.Log($"{gameObject.name}'s health changed by {amount}.");
        }

        public virtual void OnDeath()
        {
            Debug.Log($"{gameObject.name} has died.");
        }

        #endregion

        /// <summary>
        /// Method called for death animations.
        /// </summary>
        public virtual void OnDeathAnimation()
        {
            Debug.Log("Death animation played.");
        }

        /// <summary>
        /// Method called for death sounds.
        /// </summary>
        public virtual void OnDeathSound()
        {
            Debug.Log("Death sound played.");
        }

        /// <summary>
        /// Cleans up the entity.
        /// </summary>
        public void Dispose()
        {
            EntityManager.UnregisterEntity(this);

            if (NetworkServer.active)
                NetworkServer.Destroy(gameObject);
            else
                Destroy(gameObject);
        }

        protected override void OnValidate()
        {
            if (!netIdentity) return;

            base.OnValidate();
        }

        public Vector2 GetSpriteMiddlePoint()
        {
            if (!SpriteRenderer) return transform.position;

            var bounds = SpriteRenderer.sprite.bounds;
            return transform.position + bounds.center;
        }
    }

    [Serializable]
    public struct EntityData : IEquatable<EntityData>
    {
        public BaseEntity entity;
        public uint netId;

        public bool Equals(EntityData other)
        {
            return Equals(entity, other.entity) && netId == other.netId;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(entity, netId);
        }
    }
}

