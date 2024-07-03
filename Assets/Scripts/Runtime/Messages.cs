using System.Collections.Generic;
using Entity;
using Entity.Player;
using Mirror;

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
/// Informs all clients of the players connected.
/// </summary>
public struct PlayersListS2CNotify : NetworkMessage
{
    public List<PlayerSession> players;
    public List<PlayerRoleEntry> roles;

    public struct PlayerRoleEntry
    {
        public string userId;
        public PlayerRole role;
    }
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

/// <summary>
/// Server -> (broadcast) -> Client
/// Informs all clients of the current wave data.
/// </summary>
public struct WaveInfoS2CNotify : NetworkMessage
{
    public int wave;
    public long timer;
}

/// <summary>
/// Client -> Server
/// Requests the server to change the player's role.
/// </summary>
public struct ChangeRoleC2SReq : NetworkMessage
{
    public PlayerRole role;
}

/// <summary>
/// Server -> (broadcast) -> Client
/// Informs all clients that the game is over whenever a player dies.
/// </summary>
public struct GameOverS2CNotify : NetworkMessage
{

}

/// <summary>
/// Server -> (broadcast) -> Client
/// Informs all clients of the current entities in the scene.
/// </summary>
public struct SceneEntityUpdateS2CNotify : NetworkMessage
{
    public List<EntityData> entities;
}
