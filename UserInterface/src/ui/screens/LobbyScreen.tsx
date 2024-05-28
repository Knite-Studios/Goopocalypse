import { h } from "preact";

import { useEventfulState } from "onejs";

import Button from "@components/Button";
import Text, { Size } from "@components/Text";

import type { PlayerSession, ScriptManager } from "game";

function LobbyScreen({ game }: { game: ScriptManager }) {
    const { LobbyManager, GameManager } = game;

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
                { players[1] && (
                    <image
                        image={players[1].profileIcon}
                    />
                ) }

                <Text size={Size.Large}>
                    {players[1] != null ? players[1].userId?.toString() : "No Player"}
                </Text>
            </div>

            <Button
                class={"mb-4 bg-blue-500"}
                onClick={() => {
                    GameManager.StartGame();
                }}
            >
                Start Game
            </Button>
        </div>
    );
}

export default LobbyScreen;
