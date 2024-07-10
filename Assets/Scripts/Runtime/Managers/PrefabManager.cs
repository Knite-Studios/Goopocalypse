using System;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using Interfaces;
using JetBrains.Annotations;
using Mirror;
using Scriptable;
using UnityEngine;

namespace Managers
{
    public class PrefabManager : NetworkSingleton<PrefabManager>
    {
        [SerializeField] private List<Prefabs> list;

        private readonly Dictionary<PrefabType, Prefab> _prefabs = new();
        private readonly Dictionary<PrefabType, Queue<GameObject>> _pools = new();

        /// <summary>
        /// Static shortcut method for creating a prefab.
        /// </summary>
        /// <param name="prefab">The type of prefab to create.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="active">The active state of the prefab.</param>
        public static GameObject Create(PrefabType prefab, Transform parent = null, bool active = true) =>
            Instance.Instantiate(prefab, parent, active);

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Overload for creating a prefab and returning a component.
        /// </summary>
        /// <param name="prefab">The type of prefab to create.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="active">The active state of the prefab.</param>
        /// <typeparam name="T">The type of component to return.</typeparam>
        /// <returns>The component of the prefab.</returns>
        public static T Create<T>(PrefabType prefab, Transform parent = null, bool active = true) where T : Component
        {
            var newObject = Instance.Instantiate(prefab, parent, active);
            var component = newObject.GetComponent<T>();
            if (component == null)
                Debug.LogError($"Prefab {prefab} does not have component {typeof(T)}");

            return component;
        }

        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/PrefabManager");
            if (prefab == null) throw new Exception("Missing PrefabManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate PrefabManager prefab!");

            instance.name = "Managers.PrefabManager (Singleton)";
        }

        protected override void OnAwake()
        {
            DontDestroyOnLoad(this);

            foreach (var prefab in list.SelectMany(prefabList => prefabList.prefabs))
            {
                _prefabs.Add(prefab.type, prefab);

                // If the prefab should be pooled, create a pool for it.
                if (!prefab.shouldPool) continue;
                var root = GameObject.Find(prefab.root);

                _pools.Add(prefab.type, new Queue<GameObject>());
                for (var i = 0; i < prefab.initialPoolSize; i++)
                {
                    var newObject = Instantiate(prefab.prefab, root ? root.transform : transform);
                    newObject.SetActive(false);
                    _pools[prefab.type].Enqueue(newObject);
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Creates a new instance of the prefab.
        /// </summary>
        /// <param name="prefab">The prefab type</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="active">The active state.</param>
        /// <returns>The created object.</returns>
        private GameObject Instantiate(PrefabType prefab, Transform parent, bool active)
        {
            var prefabData = _prefabs[prefab];

            GameObject newObject;
            if (prefabData.shouldPool)
            {
                var pool = _pools[prefab];

                if (pool.Count > 0 && !pool.Peek().activeSelf)
                {
                    // Use the object from the pool.
                    newObject = pool.Dequeue();
                    newObject.SetActive(true);

                    // Reset the transform.
                    newObject.transform.SetParent(parent, false);
                    newObject.transform.Reset(true, true);

                    // Call reset.
                    var poolObject = newObject.GetComponent<IPoolObject>();
                    poolObject?.Reset();
                }
                else
                {
                    // Create a new object.
                    newObject = Instantiate(prefabData.prefab, parent);
                }

                // Re-add the object to the pool.
                pool.Enqueue(newObject);
            }
            else
            {
                newObject = Instantiate(prefabData.prefab, parent);
            }

            newObject.SetActive(active);

            if (prefabData.root != null)
            {
                var root = GameObject.Find(prefabData.root);
                if (root)
                {
                    newObject.transform.SetParent(root.transform, false);
                }
            }

            if (NetworkServer.active)
            {
                NetworkServer.Spawn(newObject);
            }

            return newObject;
        }

        /// <summary>
        /// Returns an object to its pool, or destroys it.
        /// </summary>
        /// <param name="prefabType">The type of prefab.</param>
        /// <param name="obj">The object to return.</param>
        public static void Destroy(PrefabType prefabType, GameObject obj)
        {
            if (!Instance._pools.ContainsKey(prefabType))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            Instance._pools[prefabType].Enqueue(obj);

            // Return the object to its pool.
            var prefab = Instance._prefabs[prefabType];
            obj.transform.SetParent(prefab.shouldPool ?
                GameObject.Find(prefab.root).transform :
                null);

            if (obj.TryGetComponent<NetworkBehaviour>(out var netObj))
            {
                if (netObj.isServer)
                    NetworkServer.Destroy(obj);
            }
        }
    }

    /// <summary>
    /// Prefab types to be instantiated.
    /// </summary>
    public enum PrefabType
    {
        None = 0,
        BasicEnemy = 1,
        DashEnemy = 2,
        LootGoblin = 3,
        RandomEnemy = 4,
        RangedEnemy = 5,
        WeepingAngelEnemy = 6,
        PlaceholderProjectile = 7,
        Link = 8,
        ArrowIndicator = 9,
        Orb = 10,
        OrbScoreText = 11,
    }

    [Serializable]
    public struct Prefab
    {
        [TitleHeader("Prefab Info")]
        public PrefabType type;
        public GameObject prefab;

        [TitleHeader("Spawning Info")]
        [CanBeNull] public string root;

        [TitleHeader("Pooling Settings")]
        public bool shouldPool;
        public int initialPoolSize;
    }
}
