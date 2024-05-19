using Entity;
using Systems.Attributes;
using XLua;

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

        #endregion

        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        protected override void ApplyBaseStats(LuaTable stats)
        {
            Name = stats.Get<string>("name");
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.AttackSpeed, stats.Get<float>("attack_speed"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }
    }
}
