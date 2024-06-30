import { h } from "preact";

import { ScreenProps } from "@ui/App";
import Box from "@components/BoxV2";
import Label from "@components/LabelV2";
import Button from "@components/ButtonV2";
import { Size } from "@components/Text";
import Setting from "@components/Setting";
import { DisplayMode } from "@type/enums";
import { useEventfulState } from "onejs";

const displayMap: { [key: string]: DisplayMode } = {
    "Fullscreen": DisplayMode.Fullscreen,
    "Borderless": DisplayMode.Borderless,
    "Windowed": DisplayMode.Windowed
};
const displayReverseMap: { [key: number]: string } = {
    [DisplayMode.Fullscreen]: "Fullscreen",
    [DisplayMode.Borderless]: "Borderless",
    [DisplayMode.Windowed]: "Windowed"
};

function SettingsScreen(props: ScreenProps) {
    const { game: { SettingsManager }, navigate } = props;

    const [fullScreen, setFullScreen]: [DisplayMode, any] = useEventfulState(SettingsManager, "Display");
    const [music, setMusicVolume]: [number, any] = useEventfulState(SettingsManager, "MusicVolume");
    const [effects, setSfxVolume]: [number, any] = useEventfulState(SettingsManager, "SoundFxVolume");

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
                <Setting current={SettingsManager.VolumeToIndex(music)}
                         name={"Music"} type={"slider"}
                         values={[0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0]}
                         onChange={(value: number) => setMusicVolume(value)}
                         class={"mb-6"}
                />

                <Setting current={SettingsManager.VolumeToIndex(effects)}
                         name={"SFX"} type={"slider"}
                         values={[0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0]}
                         onChange={(value: number) => setSfxVolume(value)}
                         class={"mb-6"}
                />

                <Setting name={"Display"} current={displayReverseMap[fullScreen]} type={"options"}
                         values={["Fullscreen", "Borderless", "Windowed"]}
                         onChange={(value: string) => setFullScreen(
                             displayMap[value] ?? DisplayMode.Fullscreen)}
                />
            </div>

            <div class={"items-center justify-center"}>
                <Button
                    size={Size.Normal}
                    class={"mb-12"}
                    onClick={() => navigate(props.lastPage)}
                >
                    Confirm?
                </Button>
            </div>
        </Box>
    );
}

export default SettingsScreen;
