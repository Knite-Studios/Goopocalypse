using System.Collections.Generic;
using Scriptable;
using UnityEngine;

namespace Entity.Player
{
    public enum PlayerRole
    {
        None,
        Buddie,
        Fwend
    }

    public static class PlayerRoleMap
    {
        private const string PlayerConfigPath = "Prefabs/Entities/Player/";

        public static readonly Dictionary<PlayerRole, PlayerConfig> Map = new();

        static PlayerRoleMap()
        {
            Map.Add(PlayerRole.Buddie, Resources.Load<PlayerConfig>(PlayerConfigPath + "Config_Buddie"));
            Map.Add(PlayerRole.Fwend, Resources.Load<PlayerConfig>(PlayerConfigPath + "Config_Fwend"));
        }
    }
}
