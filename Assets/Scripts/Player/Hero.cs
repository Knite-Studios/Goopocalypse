using Entity;
using Systems.Attributes;
using XLua;
using UnityEngine;
using Mirror;

namespace Player
{
    [CSharpCallLua]
    public class Hero : BaseEntity
    {
        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string Name { get; private set; }

        #region Attribute Getters

        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float AttackSpeed => this.GetAttributeValue<float>(Attribute.AttackSpeed);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);
        public int MaxHealth => this.GetAttributeValue<int>(Attribute.Health);

        #endregion

        /// <summary>
        /// Creates a new hero instance.
        /// </summary>
        /// <param name="luaScript">The path to the hero's Lua script.</param>
        public Hero(string luaScript) : base(luaScript)
        {
        }

        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        protected override void ApplyBaseStats(LuaTable stats)
        {
            Name = stats.Get<string>("name");
            this.GetOrCreateAttribute(Attribute.Health, stats.Get<int>("health"));
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.AttackSpeed, stats.Get<float>("attack_speed"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }

        [Command]
        public new void CmdTakeDamage(int amount)
        {
            ApplyDamage(amount);
        }

        [Server]
        private void ApplyDamage(int amount)
        {
            int finalDamage = Mathf.Max(0, amount - Armor);
            CurrentHealth -= finalDamage;
            RaiseOnDamage(finalDamage);

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnDeath();
            }

            RpcTakeDamage(finalDamage);
        }

        [ClientRpc]
        private void RpcTakeDamage(int amount)
        {
            // Client-side effects (animations, sounds, etc.)
            Debug.Log($"{Name} took {amount} damage.");
        }

        [Command]
        public new void CmdHeal(int amount)
        {
            ApplyHeal(amount);
        }

        [Server]
        private void ApplyHeal(int amount)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
            RpcHeal(amount);
        }

        [ClientRpc]
        private void RpcHeal(int amount)
        {
            // Client-side effects (animations, sounds, etc.)
            Debug.Log($"{Name} healed {amount} health.");
        }

        protected override void OnDeath()
        {
            // death handling logic
            Debug.Log($"{Name} has died.");
        }
    }

    public static class Heroes
    {
        public static Hero Fwend => new("heroes/fwend.lua");
        public static Hero Buddie => new("heroes/buddie.lua");
    }
}