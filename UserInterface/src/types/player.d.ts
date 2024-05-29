declare module "game" {
    export class PlayerSession {
        connection: NetworkConnectionToClient
        address: string
        userId: string
        profileIcon: Texture2D
    }
}
