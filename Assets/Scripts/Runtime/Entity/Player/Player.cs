using Systems.Attributes;
using XLua;
using Attribute = Systems.Attributes.Attribute;

namespace Entity.Player
{
    [CSharpCallLua]
    public class Player : BaseEntity
    {
        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string Name { get; private set; }

        #region Attribute Getters

        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

        #endregion

        protected virtual void Start()
        {
            InitializeEntityFromLua();
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

    public static class LuaPlayer
    {
        public const string Fwend = "players/fwend.lua";
        public const string Buddie = "players/buddie.lua";
    }
}
