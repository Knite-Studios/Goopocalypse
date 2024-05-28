import { h } from "preact";

import { useEventfulState } from "onejs";

import Text, { Size } from "@components/Text";

import type { PlayerSession, ScriptManager } from "game";

function LobbyScreen({ game }: { game: ScriptManager }) {
    const { LobbyManager } = game;

    const [players, _]: [PlayerSession[], any] = useEventfulState(LobbyManager, "Players");

    return (
        <div>
            <div>
                { players[0] && (
                    <image
                        image={players[0].profileIcon}
                    />
                ) }

                <Text size={Size.Large}>
                    {players[0] != null ? players[0].userId?.toString() : "No Player"}
                </Text>
            </div>

            <div>

            </div>
        </div>
    );
}

export default LobbyScreen;
