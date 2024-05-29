using System;
using JetBrains.Annotations;
using Mirror;
using Systems.Attributes;
using UnityEngine;
using XLua;
using Attribute = Systems.Attributes.Attribute;

namespace Entity.Player
{
    [CSharpCallLua]
    public class Player : BaseEntity
    {
        public PlayerRole playerRole;

        /// <summary>
        /// The name of the hero.
        /// </summary>
        public string Name { get; private set; }

        #region Attribute Getters

        public float Stamina => this.GetAttributeValue<float>(Attribute.Stamina);
        public float AreaOfEffect => this.GetAttributeValue<float>(Attribute.AreaOfEffect);

        #endregion

        private Collider2D _collider;

        protected override void Awake()
        {
            base.Awake();

            _collider = GetComponent<Collider2D>();
        }

        protected virtual void Start()
        {
            InitializePlayerConfig();
            InitializeEntityFromLua();
        }

        /// <summary>
        /// Loads the player's configuration from the scriptable object.
        /// </summary>
        /// <exception cref="Exception">Thrown when the player role is not found in the map.</exception>
        private void InitializePlayerConfig()
        {
            if (!PlayerRoleMap.Map.ContainsKey(playerRole))
                throw new Exception($"Missing player config for role: {playerRole}");

            var config = PlayerRoleMap.Map[playerRole];

            luaScript = config.luaScript;
            // animator = config.animator;
            SpriteRenderer.sprite = config.sprite;
            Rb.mass = config.mass;
            _collider.offset = config.colliderOffset;
            _collider.GetComponent<BoxCollider2D>().size = config.colliderSize;
        }

        /// <summary>
        /// Loads the statistics from a Lua script.
        /// </summary>
        /// <param name="stats">The Lua table containing the base stats.</param>
        protected override void ApplyBaseStats(LuaTable stats)
        {
            base.ApplyBaseStats(stats);

            Name = stats.Get<string>("name");
            this.GetOrCreateAttribute(Attribute.Stamina, stats.Get<float>("stamina"));
            this.GetOrCreateAttribute(Attribute.AreaOfEffect, stats.Get<float>("aoe"));
        }
    }

    [Serializable]
    public struct PlayerSession
    {
        /// <summary>
        /// This is only applicable on the server.
        /// </summary>
        [CanBeNull, NonSerialized] public NetworkConnectionToClient connection;

        public ulong userId;
        public PlayerRole role;
        [CanBeNull] public Texture2D profileIcon;
    }

    public static class LuaPlayer
    {
        public const string Fwend = "players/fwend.lua";
        public const string Buddie = "players/buddie.lua";
    }
}
