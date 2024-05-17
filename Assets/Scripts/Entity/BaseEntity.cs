using System;
using System.Collections.Generic;
using Common.Extensions;
using Managers;
using Mirror;
using Systems.Attributes;
using GameAttribute = Systems.Attributes.Attribute;
using XLua;
using UnityEngine.Events;

namespace Entity
{
    /// <summary>
    /// The base class for all entities in the game.
    /// </summary>
    public abstract class BaseEntity : NetworkManager, IAttributable
    {
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
        /// This is the maximum health of the hero.
        /// </summary>
        public int Health => this.GetAttributeValue<int>(GameAttribute.Health);
        public float Speed => this.GetAttributeValue<float>(GameAttribute.Speed);
        public int Armor => this.GetAttributeValue<int>(GameAttribute.Armor);
        public int AttackDamage => this.GetAttributeValue<int>(GameAttribute.AttackDamage);

        #endregion

        /// <summary>
        /// Event called with the damage dealt to the entity.
        /// </summary>
        public event UnityAction<int> OnDamage;

        /// <summary>
        /// Internal function caller for 'SpecialAbility' from Lua.
        /// </summary>
        protected readonly LuaSpecialAbility specialAbility;

        /// <summary>
        /// Creates a new entity instance.
        /// </summary>
        /// <param name="luaScript">The path to the entity's Lua script.</param>
        public BaseEntity(string luaScript)
        {
            var env = LuaManager.luaEnv;
            env.DoFile(luaScript);

            // Load Lua data.
            ApplyBaseStats(env.Global.Get<LuaTable>("base_stats"));
            specialAbility = env.Global.Get<LuaSpecialAbility>(LuaManager.SpecialAbilityFunc);
        }
        
        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        protected virtual void ApplyBaseStats(LuaTable stats)
        {
            this.GetOrCreateAttribute(GameAttribute.Health, stats.Get<int>("health"));
            this.GetOrCreateAttribute(GameAttribute.AttackDamage, stats.Get<int>("attack_damage"));
            this.GetOrCreateAttribute(GameAttribute.Armor, stats.Get<int>("armor"));
            this.GetOrCreateAttribute(GameAttribute.Speed, stats.Get<float>("speed"));
        }

        /// <summary>
        /// Internal function definition for the 'SpecialAbility' function.
        /// </summary>
        [CSharpCallLua]
        protected delegate void LuaSpecialAbility(BaseEntity context);

        /// <summary>
        /// Runs the hero's associated special ability.
        /// </summary>
        public void SpecialAbility() => specialAbility?.Invoke(this);

        /// <summary>
        /// Applies damage to the entity's current health.
        /// TODO: Apply armor reduction to damage.
        /// </summary>
        /// <param name="damage">The raw amount to damage.</param>
        /// <param name="trueDamage">Whether the damage is absolute. (no reductions)</param>
        public void Damage(int damage, bool trueDamage = false)
        {
            CurrentHealth -= damage;
            OnDamage?.Invoke(damage);
            
            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }

        /// <summary>
        /// Invoked when the entity dies.
        /// </summary>
        public virtual void Kill()
        {
            Destroy(gameObject); // TODO: death animation or something ??
        }
    }
}