using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "Scriptable/Spawn Data")]
    public class SpawnData : ScriptableObject
    {
        public SpawnType type;
        public List<Vector2> points;
        public List<PrefabType> enemies;
    }

    public enum SpawnType
    {
        Random,
        Points
    }
}
