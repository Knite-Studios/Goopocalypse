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
        static OnWorldGenerated: UnityAction<World>
        static OnWaveSpawn: UnityAction<number>
        State: GameState
        add_OnStateChanged(handler: (a: GameState) => void): void
        remove_OnStateChanged(handler: (a: GameState) => void): void
        OnStateChanged: OneJS.Event<(a: GameState) => void>
        constructor()
        StartDebugServer(): void
        JoinDebugServer(address: string, port: number): void
        StartGame(): void
    }

    export class LobbyManager extends MonoSingleton<LobbyManager> {
        static Initialize(): void
        transport: TransportType
        constructor()
        MakeLobby(): void
        InvitePlayer(): void
        CmdReady(): void
        StartGame(): void
    }

    export const game: ScriptManager;
}
