import { h } from "preact";

import Button from "@components/Button";
import Text, { Size } from "@components/Text";

import { PlayerRole } from "@type/enums";

import { List } from "System/Collections/Generic";
import type { PlayerSession, ScriptManager } from "game";
import { useEventfulState } from "onejs";

interface IPlayerProps {
    session: PlayerSession;
    role?: PlayerRole;
}

function Player({ session, role }: IPlayerProps) {
    return (
        <div class={"items-center mx-[50px] text-center"}>
            <image image={session.profileIcon} />

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

    const [players, _]: [List<PlayerSession>, any] = useEventfulState(
        LobbyManager,
        "Players"
    );
    const [roles, __]: [PlayerRoles, any] = useEventfulState(
        LobbyManager,
        "Roles"
    );

    return (
        <div class={"w-full h-full flex-col justify-center"}>
            <div class={"w-full flex-row justify-center mb-8"}>
                {players.ToArray().map((player, index) => (
                    <Player
                        key={index}
                        session={player}
                        role={roles[player.userId]}
                    />
                ))}
            </div>

            <div class={"w-full flex-row justify-center"}>
                <Button
                    class={"w-80 mr-8 bg-blue-500"}
                    onClick={() => {
                        GameManager.StartRemoteGame();
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
