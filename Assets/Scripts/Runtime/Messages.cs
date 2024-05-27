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
    /// Client -> Server
    /// Informs the server that the client has finished generating the world.
    /// </summary>
    public struct WorldGenDoneC2SNotify : NetworkMessage
    {

    }
}
