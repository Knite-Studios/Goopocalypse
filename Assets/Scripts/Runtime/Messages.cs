using System.Collections.Generic;
using Entity.Player;
using Mirror;

namespace Runtime
{
    /// <summary>
    /// Server -> (broadcast) -> Client
    /// All clients should transfer to the specified scene when received.
    /// </summary>
    public struct TransferSceneS2CNotify : NetworkMessage
    {
        public int sceneId;
    }

    /// <summary>
    /// Client -> Server
    /// Informs the server that the client is done loading the scene.
    /// </summary>
    public struct EnterSceneDoneC2SNotify : NetworkMessage
    {

    }

    /// <summary>
    /// Server -> (broadcast) -> Client
    /// All clients should generate the world when received.
    /// </summary>
    public struct DoWorldGenS2CReq : NetworkMessage
    {
        public int seed;
    }

    /// <summary>
    /// Client -> Server
    /// Informs the server that the client has finished generating the world.
    /// </summary>
    public struct DoWorldGenC2SRsp : NetworkMessage
    {

    }

    /// <summary>
    /// Server -> (broadcast) -> Client
    /// Informs all clients of the players connected.
    /// </summary>
    public struct PlayersListS2CNotify : NetworkMessage
    {
        public List<PlayerSession> players;
    }

    /// <summary>
    /// Server -> Client
    /// Informs a client that the login was successful.
    /// </summary>
    public struct PlayerLoginSuccessS2CNotify : NetworkMessage
    {

    }

    /// <summary>
    /// Server -> (broadcast) -> Client
    /// Informs all clients that the game has started.
    /// </summary>
    public struct GameStartS2CNotify : NetworkMessage
    {

    }
}
