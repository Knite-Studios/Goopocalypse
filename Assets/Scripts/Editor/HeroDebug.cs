using Player;
using Systems.Attributes;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class HeroDebug : EditorWindow
    {
        [MenuItem("Goopocalypse/Hero Debug")]
        private static void OpenMenu() => GetWindow<HeroDebug>().Show();

        private static Hero _hero;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Hero Debugging");

            if (GUILayout.Button("New Hero"))
            {
                _hero = Heroes.Fwend;
            }

            if (_hero != null)
            {
                EditorGUILayout.LabelField("Name", _hero.Name);
                EditorGUILayout.LabelField("Max Health", _hero.MaxHealth.ToString());
                EditorGUILayout.LabelField("Health", _hero.Health.ToString());
                EditorGUILayout.LabelField("Stamina", _hero.Stamina.ToString());
                EditorGUILayout.LabelField("Speed", _hero.Speed.ToString());
                EditorGUILayout.LabelField("Armor", _hero.Armor.ToString());
                EditorGUILayout.LabelField("Area of Effect", _hero.AreaOfEffect.ToString());

                if (GUILayout.Button("Run Special Ability"))
                {
                    _hero.SpecialAbility();
                }
                
                if (GUILayout.Button("Increase Health"))
                {
                    _hero.SetAttributeValue(Attribute.Health, _hero.Health + 10);
                }
            }
        }
    }
}
