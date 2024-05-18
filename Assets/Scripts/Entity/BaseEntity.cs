using System.Collections.Generic;
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
    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class BaseEntity : NetworkBehaviour, IAttributable
    {
        public Dictionary<GameAttribute, object> Attributes { get; } = new();
        public int CurrentHealth { get; protected set; }

        #region Attribute Getters

        public int Health => this.GetAttributeValue<int>(GameAttribute.Health);
        public float Speed => this.GetAttributeValue<float>(GameAttribute.Speed);
        public int Armor => this.GetAttributeValue<int>(GameAttribute.Armor);
        public int AttackDamage => this.GetAttributeValue<int>(GameAttribute.AttackDamage);

        #endregion

        public event UnityAction<int> OnDamage;

        private LuaSpecialAbility _specialAbility;

        public BaseEntity(string luaScript)
        {
            var env = LuaManager.luaEnv;
            env.DoFile(luaScript);

            ApplyBaseStats(env.Global.Get<LuaTable>("base_stats"));
            _specialAbility = env.Global.Get<LuaSpecialAbility>(LuaManager.SpecialAbilityFunc);
        }

        protected virtual void ApplyBaseStats(LuaTable stats)
        {
            this.GetOrCreateAttribute(GameAttribute.Health, stats.Get<int>("health"));
            this.GetOrCreateAttribute(GameAttribute.AttackDamage, stats.Get<int>("attack_damage"));
            this.GetOrCreateAttribute(GameAttribute.Armor, stats.Get<int>("armor"));
            this.GetOrCreateAttribute(GameAttribute.Speed, stats.Get<float>("speed"));
        }

        [CSharpCallLua]
        protected delegate void LuaSpecialAbility(BaseEntity context);

        public void SpecialAbility() => _specialAbility?.Invoke(this);

        [Command]
        public void CmdTakeDamage(int amount)
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
            Debug.Log($"{name} took {amount} damage.");
        }

        [Command]
        public void CmdHeal(int amount)
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
        private void RpcHeal(int amount)
        {
            // Client-side effects (animations, sounds, etc.)
            Debug.Log($"{name} healed {amount} health.");
        }

        protected void RaiseOnDamage(int amount)
        {
            OnDamage?.Invoke(amount);
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
