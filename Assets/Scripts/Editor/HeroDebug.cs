using System;
using Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class HeroDebug : EditorWindow
    {
        [MenuItem("Debug/Hero Debug")]
        private static void OpenMenu() => GetWindow<HeroDebug>().Show();

        private static HeroV2 _hero;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Hero Debugging");

            if (GUILayout.Button("New Hero"))
            {
                _hero = Heroes.Archer;
            }

            if (_hero != null)
            {
                EditorGUILayout.LabelField("Name", _hero.Name);
                EditorGUILayout.LabelField("Health", _hero.Health.ToString());
                EditorGUILayout.LabelField("Stamina", _hero.Stamina.ToString());
                EditorGUILayout.LabelField("Speed", _hero.Speed.ToString());
                EditorGUILayout.LabelField("Attack Speed", _hero.AttackSpeed.ToString());
                EditorGUILayout.LabelField("Attack Damage", _hero.AttackDamage.ToString());
                EditorGUILayout.LabelField("Armor", _hero.Armor.ToString());
                EditorGUILayout.LabelField("Area of Effect", _hero.AreaOfEffect.ToString());

                if (GUILayout.Button("Run Special Ability"))
                {
                    _hero.SpecialAbility();
                }
            }
        }
    }
}
