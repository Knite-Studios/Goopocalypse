using XLua;

namespace Player
{
    [LuaCallCSharp]
    public class Hero
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public float Stamina { get; set; }
        public float Speed { get; set; }
        public float AttackSpeed { get; set; }
        public int AttackDamage { get; set; }
        public int Armor { get; set; }
        public float AreaOfEffect { get; set; }

        [CSharpCallLua]
        protected delegate void SpecialAbilityDelegate(string name);
        protected SpecialAbilityDelegate LuaSpecialAbility;
        
        // public virtual void Move() { }
        // public virtual void Attack() { }
        public virtual void SpecialAbility() 
            => LuaSpecialAbility?.Invoke(Name);
        
        public void LoadBaseStats(LuaTable stats)
        {
            Name = stats.Get<string>("Name");
            Health = stats.Get<int>("health");
            Stamina = stats.Get<float>("stamina");
            Speed = stats.Get<float>("speed");
            AttackSpeed = stats.Get<float>("attack_speed");
            AttackDamage = stats.Get<int>("attack_damage");
            Armor = stats.Get<int>("armor");
            AreaOfEffect = stats.Get<float>("aoe");
        }
    }
}