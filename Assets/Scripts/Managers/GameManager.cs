using Player;
using UnityEngine;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private Hero _warrior, _archer, _mage;
        
        protected override void OnAwake()
        {
            InputManager.Initialize();
            LuaManager.Initialize();
        }

        private void Start()
        {
            _warrior = new Warrior();
            _archer = new Archer();
            _mage = new Mage();
            Debug.Log($"Warrior Health: {_warrior.Health}");
            Debug.Log($"Archer Health: {_archer.Health}");
            Debug.Log($"Mage Health: {_mage.Health}");
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _warrior.Name = "Garen";
                _archer.Name = "Ashe";
                _mage.Name = "Annie";
                
                _warrior.SpecialAbility();
                _archer.SpecialAbility();
                _mage.SpecialAbility();
            }
        }
    }
}