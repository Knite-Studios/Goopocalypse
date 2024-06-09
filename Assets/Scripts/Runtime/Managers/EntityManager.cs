using System;
using System.Collections.Generic;
using Entity.Enemies;
using Entity.Player;
using UnityEngine;

namespace Managers
{
    public class EntityManager : NetworkSingleton<EntityManager>
    {
        public List<PlayerController> players = new();
        public List<Enemy> enemies = new();

        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/EntityManager");
            if (prefab == null) throw new Exception("Missing EntityManager prefab!");

            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate EntityManager prefab!");

            instance.name = "Managers.EntityManager (Singleton)";
        }

        public static void RegisterPlayer(PlayerController player)
        {
            if (Instance.players.Contains(player)) return;
            Instance.players.Add(player);
        }

        public static void UnregisterPlayer(PlayerController player)
        {
            if (!Instance.players.Contains(player)) return;
            Instance.players.Remove(player);
        }

        public static void RegisterEnemy(Enemy enemy)
        {
            if (Instance.enemies.Contains(enemy)) return;
            Instance.enemies.Add(enemy);
        }

        public static void UnregisterEnemy(Enemy enemy)
        {
            if (!Instance.enemies.Contains(enemy)) return;
            Instance.enemies.Remove(enemy);
        }
    }
}
