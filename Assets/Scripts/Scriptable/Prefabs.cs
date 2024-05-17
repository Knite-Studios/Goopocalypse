using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "Scriptable", menuName = "Scriptable/Prefabs")]
    public class Prefabs : ScriptableObject
    {
        public List<Prefab> prefabs;
    }
}
