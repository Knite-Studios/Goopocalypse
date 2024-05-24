using System.Collections.Generic;
using Attributes;
using Common.Extensions;
using Managers;
using Mirror;
using Systems.Attributes;
using GameAttribute = Systems.Attributes.Attribute;
using UnityEngine;
using XLua;

namespace Entity
{
    /// <summary>
    /// The base class for all entities in the game.
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class BaseEntity : NetworkBehaviour, IAttributable, IDamageable
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
        public int CurrentHealth { get; protected set; }

        #region Attribute Getters

        public int MaxHealth => this.GetAttributeValue<int>(GameAttribute.Health);
        public float Speed => this.GetAttributeValue<float>(GameAttribute.Speed);
        public int Armor => this.GetAttributeValue<int>(GameAttribute.Armor);

        #endregion

        /// <summary>
        /// Internal function caller for 'SpecialAbility' from Lua.
        /// </summary>
        private LuaSpecialAbility _specialAbility;

        /// <summary>
        /// Loads the entity's data from a Lua script.
        /// </summary>
        protected void InitializeEntityFromLua()
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
        private delegate void LuaSpecialAbility(BaseEntity context);

        /// <summary>
        /// Invoked when the object is destroyed.
        /// We unset the ability so our Lua environment can destroy peacefully.
        /// </summary>
        private void OnDisable() => _specialAbility = null;

        /// <summary>
        /// Runs the entity's associated special ability.
        /// </summary>
        public void SpecialAbility() => _specialAbility?.Invoke(this);

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

        [ClientRpc]
        public virtual void OnDeath()
        {
            // TODO: Play a client-side death animation.
            Debug.Log($"{gameObject.name} has died.");
        }

        #endregion
    }
}
