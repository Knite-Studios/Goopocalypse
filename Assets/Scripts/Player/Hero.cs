using System.Collections.Generic;
using Systems.Attributes;
using XLua;

namespace Player
{
    [LuaCallCSharp]
    public class Hero : IAttributable
    {
        public string Name { get; set; }
        public int Health => this.GetAttributeValue<int>(Attribute.Health);
        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float Speed => this.GetAttributeValue<float>(Attribute.Speed);
        public float AttackSpeed => this.GetAttributeValue<float>(Attribute.AttackSpeed);
        public int AttackDamage => this.GetAttributeValue<int>(Attribute.AttackDamage);
        public int Armor => this.GetAttributeValue<int>(Attribute.Armor);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

        [CSharpCallLua]
        protected delegate void SpecialAbilityDelegate(string name);
        protected SpecialAbilityDelegate LuaSpecialAbility;

        public virtual void SpecialAbility() => LuaSpecialAbility?.Invoke(Name);

        /// <summary>
        /// Loads the base stats of the hero from a Lua table.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        public void LoadBaseStats(LuaTable stats)
        {
            Name = stats.Get<string>("name");
            this.GetOrCreateAttribute(Attribute.Health, stats.Get<float>("health"));
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.Speed, stats.Get<float>("speed"));
            this.GetOrCreateAttribute(Attribute.AttackSpeed, stats.Get<float>("attack_speed"));
            this.GetOrCreateAttribute(Attribute.AttackDamage, stats.Get<float>("attack_damage"));
            this.GetOrCreateAttribute(Attribute.Armor, stats.Get<float>("armor"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }

        public Dictionary<Attribute, object> Attributes { get; } = new();
    }
}
