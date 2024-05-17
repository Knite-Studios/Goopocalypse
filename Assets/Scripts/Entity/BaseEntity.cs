using System.Collections.Generic;
using Common.Extensions;
using Managers;
using Mirror;
using Systems.Attributes;
using UnityEngine;
using XLua;

namespace Entity
{
    /// <summary>
    /// The base class for all entities in the game.
    /// </summary>
    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class BaseEntity : NetworkBehaviour, IAttributable
    {
        /// <summary>
        /// Attribute holder map.
        /// </summary>
        public Dictionary<Attribute, object> Attributes { get; } = new();

        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public int CurrentHealth { get; set; }

        #region Attribute Getters

        /// <summary>
        /// This is the maximum health of the hero.
        /// </summary>
        public int Health => this.GetAttributeValue<int>(Attribute.Health);
        public float Speed => this.GetAttributeValue<float>(Attribute.Speed);
        public int Armor => this.GetAttributeValue<int>(Attribute.Armor);
        public int AttackDamage => this.GetAttributeValue<int>(Attribute.AttackDamage);

        #endregion

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
            this.GetOrCreateAttribute(Attribute.Health, stats.Get<int>("health"));
            this.GetOrCreateAttribute(Attribute.AttackDamage, stats.Get<int>("attack_damage"));
            this.GetOrCreateAttribute(Attribute.Armor, stats.Get<int>("armor"));
            this.GetOrCreateAttribute(Attribute.Speed, stats.Get<float>("speed"));
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
    }
}