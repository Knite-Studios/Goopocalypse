using System.Collections.Generic;
using Common.Extensions;
using Managers;
using Systems.Attributes;
using XLua;

namespace Player
{
    [CSharpCallLua]
    public class HeroV2 : IAttributable
    {
        /// <summary>
        /// Attribute holder map.
        /// </summary>
        public Dictionary<Attribute, object> Attributes { get; } = new();

        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The current health of the hero.
        /// </summary>
        public int CurrentHealth;

        #region Attribute Getters

        /// <summary>
        /// This is the maximum health of the hero.
        /// </summary>
        public int Health => this.GetAttributeValue<int>(Attribute.Health);
        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float Speed => this.GetAttributeValue<float>(Attribute.Speed);
        public float AttackSpeed => this.GetAttributeValue<float>(Attribute.AttackSpeed);
        public int AttackDamage => this.GetAttributeValue<int>(Attribute.AttackDamage);
        public int Armor => this.GetAttributeValue<int>(Attribute.Armor);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

        #endregion

        /// <summary>
        /// Internal function caller for 'SpecialAbility' from Lua.
        /// </summary>
        private readonly LuaSpecialAbility _specialAbility;

        /// <summary>
        /// Creates a new hero instance.
        /// </summary>
        /// <param name="luaScript">The path to the hero's Lua script.</param>
        public HeroV2(string luaScript)
        {
            var env = LuaManager.luaEnv;
            env.DoFile(luaScript);

            // Load Lua data.
            ApplyBaseStats(env.Global.Get<LuaTable>("base_stats"));
            _specialAbility = env.Global.Get<LuaSpecialAbility>(LuaManager.SpecialAbilityFunc);
        }

        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        private void ApplyBaseStats(LuaTable stats)
        {
            Name = stats.Get<string>("name");
            this.GetOrCreateAttribute(Attribute.Health, stats.Get<int>("health"));
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.Speed, stats.Get<float>("speed"));
            this.GetOrCreateAttribute(Attribute.AttackSpeed, stats.Get<float>("attack_speed"));
            this.GetOrCreateAttribute(Attribute.AttackDamage, stats.Get<int>("attack_damage"));
            this.GetOrCreateAttribute(Attribute.Armor, stats.Get<int>("armor"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }

        /// <summary>
        /// Internal function definition for the 'SpecialAbility' function.
        /// </summary>
        [CSharpCallLua]
        private delegate void LuaSpecialAbility(HeroV2 context);

        /// <summary>
        /// Runs the hero's associated special ability.
        /// </summary>
        public void SpecialAbility() => _specialAbility?.Invoke(this);
    }

    /// <summary>
    /// DEVELOPERS NOTE: This is an example of how to replace the existing hardcoded classes.
    /// </summary>
    public static class Heroes
    {
        public static HeroV2 Warrior => new("warrior.lua");
        public static HeroV2 Mage => new("mage.lua");
        public static HeroV2 Archer => new("archer.lua");
    }
}
