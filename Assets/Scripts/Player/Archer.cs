using Managers;
using UnityEngine;
using XLua;

namespace Player
{
    [LuaCallCSharp]
    public class Archer : Hero
    {
        private LuaTable _table;

        public Archer()
        {
            var luaEnv = LuaManager.luaEnv;
            LuaManager.Instance.DoFile("archer");
            _table = luaEnv.Global.Get<LuaTable>("BaseStats");
            LoadBaseStats(_table);
            LuaSpecialAbility = luaEnv.Global.Get<SpecialAbilityDelegate>("OnSpecialAbility");
        }

        public override void SpecialAbility()
        {
            base.SpecialAbility();
            Debug.Log("All the world on one arrow!");
        }
    }
}