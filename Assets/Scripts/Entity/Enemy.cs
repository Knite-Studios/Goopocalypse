using XLua;

namespace Entity
{
    [CSharpCallLua]
    public class Enemy : BaseEntity
    {
        /// <summary>
        /// Creates a new enemy instance.
        /// </summary>
        /// <param name="luaScript">The path to the enemy's Lua script.</param>
        public Enemy(string luaScript) : base(luaScript)
        {

        }
    }

    public static class Enemies
    {
        public static Enemy MeleeEnemy => new("enemies/melee_enemy.lua");
    }
}