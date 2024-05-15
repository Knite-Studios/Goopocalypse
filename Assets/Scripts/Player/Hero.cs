using Systems.Attributes;
using XLua;

namespace Player
{
    [LuaCallCSharp]
    public class Hero : Attributable
    {
        public string Name { get; set; }
        public int Health => GetAttributeValue<int>(Attribute.Health);
        public float Stamina => GetAttributeValue<float>(Attribute.Stamina);
        public float Speed => GetAttributeValue<float>(Attribute.Speed);
        public float AttackSpeed => GetAttributeValue<float>(Attribute.AttackSpeed);
        public int AttackDamage => GetAttributeValue<int>(Attribute.AttackDamage);
        public int Armor => GetAttributeValue<int>(Attribute.Armor);
        public float AreaOfEffect => GetAttributeValue<float>(Attribute.AreaOfEffect);

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
            GetOrCreateAttribute(Attribute.Health, stats.Get<float>("health"));
            GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            GetOrCreateAttribute(Attribute.Speed, stats.Get<float>("speed"));
            GetOrCreateAttribute(Attribute.AttackSpeed, stats.Get<float>("attack_speed"));
            GetOrCreateAttribute(Attribute.AttackDamage, stats.Get<float>("attack_damage"));
            GetOrCreateAttribute(Attribute.Armor, stats.Get<float>("armor"));
            GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }
    }
}
