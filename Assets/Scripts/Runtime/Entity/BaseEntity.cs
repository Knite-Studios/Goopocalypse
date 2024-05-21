using System;
using System.Collections.Generic;
using Attributes;
using Common.Extensions;
using Managers;
using Mirror;
using Systems.Attributes;
using GameAttribute = Systems.Attributes.Attribute;
using UnityEngine;
using XLua;
using UnityEngine.Events;

namespace Entity
{
    /// <summary>
    /// The base class for all entities in the game.
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class BaseEntity : NetworkBehaviour, IAttributable
    {
        /// <summary>
        /// The Lua script responsible for the logic of the entity.
        /// </summary>
        [TitleHeader("Entity Settings")]
        public string luaScript;

        /// <summary>
        /// Attribute holder map.
        /// </summary>
        public Dictionary<GameAttribute, object> Attributes { get; } = new();

        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public int CurrentHealth { get; set; }

        #region Attribute Getters

        /// <summary>
        /// This is the maximum health of the entity.
        /// </summary>
        public int Health => this.GetAttributeValue<int>(GameAttribute.Health);
        public int MaxHealth => this.GetAttributeValue<int>(GameAttribute.Health);
        public float Speed => this.GetAttributeValue<float>(GameAttribute.Speed);
        public int Armor => this.GetAttributeValue<int>(GameAttribute.Armor);

        #endregion

        /// <summary>
        /// Event called with the damage dealt to the entity.
        /// </summary>
        public event UnityAction<int> OnDamage;

        /// <summary>
        /// Internal function caller for 'SpecialAbility' from Lua.
        /// </summary>
        private LuaSpecialAbility _specialAbility;

        public void InitializeEntityFromLua()
        {
            var env = ScriptManager.Environment;
            env.DoFile(luaScript);

            // Load Lua data.
            ApplyBaseStats(env.Global.Get<LuaTable>("base_stats"));
            _specialAbility = env.Global.Get<LuaSpecialAbility>(ScriptManager.SpecialAbilityFunc);
        }

        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        protected virtual void ApplyBaseStats(LuaTable stats)
        {
            this.GetOrCreateAttribute(GameAttribute.Health, stats.Get<int>("health"));
            this.GetOrCreateAttribute(GameAttribute.MaxHealth, stats.Get<int>("max_health"));
            this.GetOrCreateAttribute(GameAttribute.Speed, stats.Get<float>("speed"));
            this.GetOrCreateAttribute(GameAttribute.Armor, stats.Get<int>("armor"));
        }

        /// <summary>
        /// Internal function definition for the 'SpecialAbility' function.
        /// </summary>
        [CSharpCallLua]
        protected delegate void LuaSpecialAbility(BaseEntity context);

        /// <summary>
        /// Runs the entity's associated special ability.
        /// </summary>
        public void SpecialAbility() => _specialAbility?.Invoke(this);

        [Command]
        public virtual void CmdTakeDamage(int damage)
        {
            ApplyDamage(damage);
        }

        /// <summary>
        /// Applies damage to the entity's current health.
        /// </summary>
        /// <param name="damage">The raw amount to damage.</param>
        /// <param name="trueDamage">Whether the damage is absolute. (no reductions)</param>
        [Server]
        private void ApplyDamage(int damage, bool trueDamage = false)
        {
            var finalDamage = trueDamage ? damage : Mathf.Max(0, damage - Armor);
            CurrentHealth -= finalDamage;
            OnDamage?.Invoke(damage);

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnDeath();
            }

            RpcTakeDamage(finalDamage);
        }

        [ClientRpc]
        protected virtual void RpcTakeDamage(int amount)
        {
            // Client-side effects (animations, sounds, etc.).
            Debug.Log($"{name} took {amount} damage.");
        }

        [Command]
        public virtual void CmdHeal(int amount)
        {
            ApplyHeal(amount);
        }

        [Server]
        private void ApplyHeal(int amount)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + amount, Health);
            RpcHeal(amount);
        }

        [ClientRpc]
        protected virtual void RpcHeal(int amount)
        {
            // Client-side effects (animations, sounds, etc.).
            Debug.Log($"{name} healed {amount} health.");
        }

        protected virtual void OnDeath()
        {
            // Death logic
            Debug.Log($"{name} has died.");
        }

        public virtual void Kill()
        {
            Destroy(gameObject); // TODO: death animation or other logic
        }
    }
}
