import { h } from "preact";

import { useEventfulState } from "onejs";

import Button from "@components/Button";
import Text, { Size } from "@components/Text";

import type { PlayerSession, ScriptManager } from "game";

import { PlayerRole } from "@types/enums";
import { List } from "System/Collections/Generic";

interface IPlayerProps {
    session: PlayerSession;
    role?: PlayerRole;
}

function Player({ session, role }: IPlayerProps) {
    log(`User ID: ${session.userId}, Role: ${role}`);

    return (
        <div class={"flex items-center mx-[50px] text-center"}>
            <image
                image={session.profileIcon}
            />

            <Text size={Size.Large}>
                {session.userId ?? "No ID"}
                {role?.toString() ?? "No Role"}
            </Text>
        </div>
    );
}

type PlayerRoles = { [key: string]: PlayerRole };

function LobbyScreen({ game }: { game: ScriptManager }) {
    const { LobbyManager, GameManager } = game;

    const [players, _]: [List<PlayerSession>, any] = useEventfulState(LobbyManager, "Players");
    const [roles, __]: [PlayerRoles, any] = useEventfulState(LobbyManager, "Roles");

    return (
        <div class={"w-full h-full flex flex-col justify-center"}>
            <div class={"w-full flex flex-row justify-center mb-8"}>
                { players.ToArray().map((player, index) => (
                    <Player
                        key={index}
                        session={player}
                        role={roles[player.userId]}
                    />
                )) }
            </div>

            <div class={"w-full flex flex-row justify-center"}>
                <Button
                    class={"w-64 mr-8 bg-blue-500"}
                    onClick={() => {
                        GameManager.StartGame();
                    }}
                >
                    Start Game
                </Button>

                <Button
                    class={"mr-8 bg-blue-500"}
                    onClick={() => {
                        GameManager.ChangeRole(PlayerRole.Fwend);
                    }}
                >
                    Change to Fwend
                </Button>

                <Button
                    class={"bg-blue-500"}
                    onClick={() => {
                        GameManager.ChangeRole(PlayerRole.Buddie);
                    }}
                >
                    Change to Buddie
                </Button>
            </div>
        </div>
    );
}

export default LobbyScreen;
