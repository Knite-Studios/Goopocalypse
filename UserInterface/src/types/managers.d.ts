declare module "game" {
    export class WaveManager extends NetworkSingleton<WaveManager> {
        NetworkwaveCount: number
        NetworkmatchTimer: number
        waveCount: number
        matchTimer: number
        constructor()
        Weaved(): boolean
        SerializeSyncVars(writer: NetworkWriter, forceAll: boolean): void
        DeserializeSyncVars(reader: NetworkReader, initialState: boolean): void
    }

    export class ScriptManager extends MonoSingleton<ScriptManager> {
        static Environment: LuaEnv
        static LuaRoot: string
        static SpecialAbilityFunc: string
        static BehaviorUpdateFunc: string
        WaveManager: WaveManager
        GameManager: GameManager
        LobbyManager: LobbyManager
        constructor()
    }

    export class GameManager extends MonoSingleton<GameManager> {
        static ScriptEngine: ScriptEngine
        static OnGameStart: () => void
        static OnGameEvent: UnityAction<GameEvent>
        static OnWaveSpawn: UnityAction<number>
        ProfilePicture: Texture2D
        Username: string
        State: GameState
        add_OnRouteUpdate(handler: (a: string) => void): void
        remove_OnRouteUpdate(handler: (a: string) => void): void
        OnRouteUpdate: OneJS.Event<(a: string) => void>
        add_OnProfilePictureChanged(handler: (a: Texture2D) => void): void
        remove_OnProfilePictureChanged(handler: (a: Texture2D) => void): void
        OnProfilePictureChanged: OneJS.Event<(a: Texture2D) => void>
        add_OnUsernameChanged(handler: (a: string) => void): void
        remove_OnUsernameChanged(handler: (a: string) => void): void
        OnUsernameChanged: OneJS.Event<(a: string) => void>
        add_OnStateChanged(handler: (a: GameState) => void): void
        remove_OnStateChanged(handler: (a: GameState) => void): void
        OnStateChanged: OneJS.Event<(a: GameState) => void>
        gameScene: number
        DefaultRoute: string
        constructor()
        StartDebugServer(): void
        JoinDebugServer(address: string, port: number): void
        StartGame(): void
        ChangeRole(role: PlayerRole): void
        Navigate(path: string): void
    }

    export class LobbyManager extends MonoSingleton<LobbyManager> {
        static OnPlayerConnected: (a: NetworkConnectionToClient) => void
        static OnPlayerDisconnected: (a: NetworkConnectionToClient) => void
        static Initialize(): void
        Players: List<PlayerSession>
        Roles: Dictionary<string, PlayerRole>
        add_OnPlayersChanged(handler: (a: List<PlayerSession>) => void): void
        remove_OnPlayersChanged(handler: (a: List<PlayerSession>) => void): void
        OnPlayersChanged: OneJS.Event<(a: List<PlayerSession>) => void>
        add_OnRolesChanged(handler: (a: Dictionary<string, PlayerRole>) => void): void
        remove_OnRolesChanged(handler: (a: Dictionary<string, PlayerRole>) => void): void
        OnRolesChanged: OneJS.Event<(a: Dictionary<string, PlayerRole>) => void>
        transport: TransportType
        constructor()
        MakeLobby(): void
        CloseLobby(): void
        InvitePlayer(): void
    }

    export const game: ScriptManager;
}
