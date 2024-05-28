declare module "game" {
    export class PlayerSession {
        connection: NetworkConnectionToClient
        userId: number | null
        role: PlayerRole | null
        profileIcon: Texture2D
    }
}
