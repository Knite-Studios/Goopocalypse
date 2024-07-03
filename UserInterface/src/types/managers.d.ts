declare module "game" {
    export class WaveManager extends MonoSingleton<WaveManager> {
        WaveCount: number
        MatchTimer: number
        Score: number
        add_OnWaveCountChanged(handler: (a: number) => void): void
        remove_OnWaveCountChanged(handler: (a: number) => void): void
        OnWaveCountChanged: OneJS.Event<(a: number) => void>
        add_OnMatchTimerChanged(handler: (a: number) => void): void
        remove_OnMatchTimerChanged(handler: (a: number) => void): void
        OnMatchTimerChanged: OneJS.Event<(a: number) => void>
        add_OnScoreChanged(handler: (a: number) => void): void
        remove_OnScoreChanged(handler: (a: number) => void): void
        OnScoreChanged: OneJS.Event<(a: number) => void>
        spawnThreshold: number
        constructor()
        SpawnWave(): void
    }

    export class ScriptManager extends MonoSingleton<ScriptManager> {
        static Environment: LuaEnv;
        static LuaRoot: string;
        static SpecialAbilityFunc: string;
        static BehaviorUpdateFunc: string;
        WaveManager: WaveManager;
        GameManager: GameManager;
        LobbyManager: LobbyManager;
        SettingsManager: SettingsManager;
        AudioManager: AudioManager;
        constructor();
    }

    export class GameManager extends MonoSingleton<GameManager> {
        static ScriptEngine: ScriptEngine
        static OnGameStart: () => void
        static OnGameEvent: UnityAction<GameEvent>
        static OnWaveSpawn: UnityAction<number>
        static OnGameOver: () => void
        ProfilePicture: Texture2D
        Username: string
        LocalMultiplayer: boolean
        State: GameState
        Route: string
        LoadingProgress: number
        add_OnProfilePictureChanged(handler: (a: Texture2D) => void): void
        remove_OnProfilePictureChanged(handler: (a: Texture2D) => void): void
        OnProfilePictureChanged: OneJS.Event<(a: Texture2D) => void>
        add_OnUsernameChanged(handler: (a: string) => void): void
        remove_OnUsernameChanged(handler: (a: string) => void): void
        OnUsernameChanged: OneJS.Event<(a: string) => void>
        add_OnLocalMultiplayerChanged(handler: (a: boolean) => void): void
        remove_OnLocalMultiplayerChanged(handler: (a: boolean) => void): void
        OnLocalMultiplayerChanged: OneJS.Event<(a: boolean) => void>
        add_OnStateChanged(handler: (a: GameState) => void): void
        remove_OnStateChanged(handler: (a: GameState) => void): void
        OnStateChanged: OneJS.Event<(a: GameState) => void>
        add_OnRouteChanged(handler: (a: string) => void): void
        remove_OnRouteChanged(handler: (a: string) => void): void
        OnRouteChanged: OneJS.Event<(a: string) => void>
        add_OnLoadingProgressChanged(handler: (a: number) => void): void
        remove_OnLoadingProgressChanged(handler: (a: number) => void): void
        OnLoadingProgressChanged: OneJS.Event<(a: number) => void>
        menuScene: number
        gameScene: number
        constructor()
        StartDebugServer(): void
        JoinDebugServer(address: string, port: number): void
        StartRemoteGame(): void
        StartLocalGame(): void
        ChangeRole(role: PlayerRole): void
        ResumeGame(): void
        RestartGame(): void
        StopGame(): void
        Navigate(path: string): void
        QuitGame(): void
        LoadScene(sceneId: number): Task<AsyncOperation>
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
        FindPlayer(conn: NetworkConnectionToClient): PlayerSession
        GetPlayerRole(conn: NetworkConnectionToClient): PlayerRole
        LeaveLobby(): void
        MakeLobby(): void
        CloseLobby(): void
        InvitePlayer(): void
    }

    export class SettingsManager extends MonoSingleton<SettingsManager> {
        static Initialize(): void
        MusicVolume: number
        SoundFxVolume: number
        Display: DisplayMode
        add_OnMusicVolumeChanged(handler: (a: number) => void): void
        remove_OnMusicVolumeChanged(handler: (a: number) => void): void
        OnMusicVolumeChanged: OneJS.Event<(a: number) => void>
        add_OnSoundFxVolumeChanged(handler: (a: number) => void): void
        remove_OnSoundFxVolumeChanged(handler: (a: number) => void): void
        OnSoundFxVolumeChanged: OneJS.Event<(a: number) => void>
        add_OnDisplayChanged(handler: (a: DisplayMode) => void): void
        remove_OnDisplayChanged(handler: (a: DisplayMode) => void): void
        OnDisplayChanged: OneJS.Event<(a: DisplayMode) => void>
        audioMixer: AudioMixer
        constructor()
        VolumeToIndex(volume: number): number
        PlayUIHoverSound(): void
        PlayUIClickSound(): void
        SetDisplayMode(mode: DisplayMode): void
    }

    export class AudioManager extends MonoSingleton<AudioManager> {
        PlayUIHoverSound(): void
        PlayUIClickSound(): void
    }

    export const game: ScriptManager;
}
