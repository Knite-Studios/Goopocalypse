import { h } from "preact";

import { ScreenProps } from "@ui/App";
import Box from "@components/BoxV2";
import Label from "@components/LabelV2";
import Button from "@components/ButtonV2";
import { Size } from "@components/Text";
import Setting from "@components/Setting";
import { GameState, DisplayMode } from "@type/enums";

function SettingsScreen(props: ScreenProps) {
    const { game: { GameManager }, navigate } = props;
    const isLobby = GameManager.State == GameState.Lobby;

    return (
        <Box
            overlay
            class={"flex-col justify-between"}
        >
            <div class={"flex-row mt-12"}>
                <Label>
                    Settings
                </Label>

                <div />
            </div>

            <div class={"flex-col px-32"}>
                <Setting name={"Music"} current={5} type={"slider"}
                         values={[10, 20, 30, 40, 50, 60, 70, 80, 90, 100]}
                         onChange={(value) => log(`new value: ${value}`)}
                         class={"mb-6"}
                />

                <Setting name={"SFX"} current={5} type={"slider"}
                         values={[10, 20, 30, 40, 50, 60, 70, 80, 90, 100]}
                         onChange={(value) => log(`new value: ${value}`)}
                         class={"mb-6"}
                />

                <Setting name={"Display"} current={"Borderless"} type={"options"}
                         values={["Fullscreen", "Borderless", "Windowed"]}
                         onChange={(value) => log(`new value: ${value}`)}
                />
            </div>

            <div class={"items-center justify-center"}>
                <Button
                    size={Size.Normal}
                    class={"mb-12"}
                    onClick={() => {
                        // TODO: Save settings with C#.
                        navigate(props.lastPage);
                    }}
                >
                    Confirm?
                </Button>
            </div>
        </Box>
    );
}

export default SettingsScreen;
