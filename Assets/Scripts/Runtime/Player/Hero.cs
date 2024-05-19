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
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

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
            this.GetOrCreateAttribute(Attribute.MaxHealth, stats.Get<int>("max_health"));
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.Speed, stats.Get<float>("speed"));
            this.GetOrCreateAttribute(Attribute.Armor, stats.Get<int>("armor"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }
    }

    public static class Heroes
    {
        public static Hero Fwend => new("heroes/fwend.lua");
        public static Hero Buddie => new("heroes/buddie.lua");
    }
}