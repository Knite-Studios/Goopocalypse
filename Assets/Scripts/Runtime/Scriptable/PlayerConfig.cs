using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "Scriptable/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Identity")]
        public string playerName;

        [Header("Visual")]
        public RuntimeAnimatorController animatorController;
        public Sprite sprite;
        public float mass;
        public int sortingOrder = 10;

        [Header("Spawn")]
        public Vector2 spawnPoint = new(0, 0);

        [Header("Core Stats")]
        [Min(1)] public int maxHealth = 100;
        [Min(0f)] public float speed = 4f;
        [Min(0)] public int armor = 8;

        [Header("Player Stats")]
        [Min(0f)] public float stamina = 100f;
        [Min(0f)] public float areaOfEffect = 3f;
        [Min(0f)] public float cameraDistance = 6.5f;
    }
}
