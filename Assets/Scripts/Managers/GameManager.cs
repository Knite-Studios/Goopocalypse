using Player;
using Systems.Attributes;
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
            
            _warrior.Name = "Garen";
            _archer.Name = "Ashe";
            _mage.Name = "Annie";
            
            Debug.Log($"Warrior Health: {_warrior.GetAttributeValue<float>(Attribute.Health)}");
            Debug.Log($"Archer Health: {_archer.GetAttributeValue<float>(Attribute.Health)}");
            Debug.Log($"Mage Health: {_mage.GetAttributeValue<float>(Attribute.Health)}");
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                // _warrior.SpecialAbility();
                // _archer.SpecialAbility();
                // _mage.SpecialAbility();
                
                Debug.Log($"Current Health: {_warrior.GetAttributeValue<float>(Attribute.Health)}");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                var newHealth = _warrior.GetAttributeValue<float>(Attribute.Health) + 10;
                SetHeroHealth(_warrior, newHealth);
                Debug.Log($"New Health: {_warrior.GetAttributeValue<float>(Attribute.Health)}");
            }
        }
        
        private void SetHeroHealth(Hero hero, float newHealth)
        {
            var healthAttribute = hero.GetOrCreateAttribute<float>(Attribute.Health, newHealth);
            healthAttribute.BaseValue = newHealth;
        }
    }
}