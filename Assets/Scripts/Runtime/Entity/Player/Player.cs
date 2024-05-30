using System;
using System.Linq;
using JetBrains.Annotations;
using Managers;
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
        [SyncVar] public PlayerRole playerRole;
        public Transform rope;

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
        /// Temporary method for prototyping.
        /// </summary>
        private void InitializeRope()
        {
            switch (playerRole)
            {
                case PlayerRole.Fwend:
                    var buddie = (from player in LobbyManager.Instance.Players
                        select player.connection!.identity.GetComponent<PlayerController>()
                        into controller where controller.playerRole == PlayerRole.Buddie select controller)
                        .FirstOrDefault();

                    var lastSegment = buddie!.rope.childCount - 1;
                    var lastSegmentRigidbody = transform.GetChild(lastSegment).GetComponent<Rigidbody2D>();
                    var joint = gameObject.GetOrAddComponent<HingeJoint2D>();
                    joint.connectedBody = lastSegmentRigidbody;
                    break;
                case PlayerRole.Buddie:
                    break;
            }
        }

        /// <summary>
        /// Loads the player's configuration from the scriptable object.
        /// </summary>
        /// <exception cref="Exception">Thrown when the player role is not found in the map.</exception>
        private void InitializePlayerConfig()
        {
            if (!PlayerRoleMap.Map.TryGetValue(playerRole, out var config))
                throw new Exception($"Missing player config for role: {playerRole}");

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

        public string address, userId;
        [CanBeNull] public Texture2D profileIcon;
    }

    public static class LuaPlayer
    {
        public const string Fwend = "players/fwend.lua";
        public const string Buddie = "players/buddie.lua";
    }
}
