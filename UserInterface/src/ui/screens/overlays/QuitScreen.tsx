import { h } from "preact";

import { ScreenProps } from "@ui/App";
import { GameState } from "@type/enums";
import Box from "@components/BoxV2";
import Label from "@components/LabelV2";
import Text, { Size } from "@components/Text";
import Button from "@components/ButtonV2";
import { Application } from "UnityEngine";

function QuitScreen({ game, navigate }: ScreenProps) {
    const { GameManager } = game;

    const isLobby = GameManager.State == GameState.Lobby;

    return (
        <Box
            overlay
            class={"flex-col justify-between"}
        >
            <div class={"flex-row mt-12"}>
                <Label>
                    Quit Game
                </Label>

                <div />
            </div>

            <Text size={Size.Large} class={"self-center text-lime"}>Leaving Already?</Text>

            <div class={"w-full flex-row justify-between px-24 pb-8"}>
                <Button
                    class={"w-[49%]"}
                    onClick={() => {
                        if (isLobby) {
                            Application.Quit();
                        } else {
                            navigate("/");
                            // TODO: Call the game's stop method.
                        }
                    }}
                >
                    Confirm?
                </Button>

                <div class={"mr-[2%]"} />

                <Button
                    bounce={false}
                    class={"w-[49%]"}
                    defaultColor={"#51d598"}
                    onClick={() => navigate(isLobby ? "/" : "/game/pause")}
                >
                    Back
                </Button>
            </div>
        </Box>
    );
}

export default QuitScreen;
