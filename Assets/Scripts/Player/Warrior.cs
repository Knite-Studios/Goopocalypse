using Managers;
using UnityEngine;
using XLua;

namespace Player
{
    [LuaCallCSharp]
    public class Warrior : Hero
    {
        private LuaTable _table;

        public Warrior()
        {
            var luaEnv = LuaManager.luaEnv;
            LuaManager.Instance.DoFile("warrior");
            _table = luaEnv.Global.Get<LuaTable>("BaseStats");
            LoadBaseStats(_table);
            LuaSpecialAbility = luaEnv.Global.Get<SpecialAbilityDelegate>("OnSpecialAbility");
        }
        
        public override void SpecialAbility()
        {
            base.SpecialAbility();
            Debug.Log("My heart and sword always for Demacia!");
        }
    }
}