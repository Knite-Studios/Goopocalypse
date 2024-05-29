using System.Collections.Generic;

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
        public static readonly Dictionary<PlayerRole, string> Map = new();

        static PlayerRoleMap()
        {
            Map.Add(PlayerRole.Buddie, LuaPlayer.Buddie);
            Map.Add(PlayerRole.Fwend, LuaPlayer.Fwend);
        }
    }
}
