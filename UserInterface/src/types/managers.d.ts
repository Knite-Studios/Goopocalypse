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
        WaveManager: WaveManager
        GameManager: GameManager
        constructor()
    }

    export class GameManager extends MonoSingleton<GameManager> {
        static ScriptEngine: ScriptEngine
        static OnGameStart: () => void
        static OnGameEvent: UnityAction<GameEvent>
        scriptEngine: ScriptEngine
        networkManager: NetworkManager
        constructor()
        StartDebugServer(): void
        JoinDebugServer(address: string, port: number): void
        StartGame(): void
    }

    export const game: ScriptManager;
}
