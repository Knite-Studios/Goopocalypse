using Managers;
using UnityEngine;
using XLua;

namespace Player
{
    [LuaCallCSharp]
    public class Mage : Hero
    {
        private LuaTable _table;

        public Mage()
        {
            var luaEnv = LuaManager.luaEnv;
            LuaManager.Instance.DoFile("mage");
            _table = luaEnv.Global.Get<LuaTable>("BaseStats");
            LoadBaseStats(_table);
            LuaSpecialAbility = luaEnv.Global.Get<SpecialAbilityDelegate>("OnSpecialAbility");
        }

        public override void SpecialAbility()
        {
            base.SpecialAbility();
            Debug.Log("Yay, Tibbers!");
        }
    }
}