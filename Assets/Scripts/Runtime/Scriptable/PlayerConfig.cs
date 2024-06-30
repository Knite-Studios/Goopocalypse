using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "Scriptable/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        public string luaScript;
        public RuntimeAnimatorController animatorController;
        public Sprite sprite;
        public float mass;
        public Vector2 colliderOffset;
        public Vector2 colliderSize;
        public int sortingOrder = 10;
    }
}
