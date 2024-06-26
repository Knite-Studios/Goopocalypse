import { h, JSX } from "preact";

import Label from "@components/Label";
import TextBox from "@components/TextBox";

import { MenuState } from "@type/enums";

import type { ScreenProps } from "@ui/App";
import * as resources from "@ui/resources";
import { Size } from "@components/Text";

interface PlayerCardProps {
    index: number;

    class?: string;
}

function PlayerCard(props: PlayerCardProps) {
    return (
        <div class={`${props.class} flex-col`}>
            <div class={`w-full h-full flex-col`}>
                <TextBox
                    labelSize={Size.Large}
                    positioning={{ rotate: -3.81 }}
                    label={`Player ${props.index + 1}`}
                />
                <div class={"mb-14"} />

                <image
                    class={"mb-4"}
                    image={resources.PlayerPlaceholder}
                />

                <div class={"justify-center self-center flex-row"}>
                    <image
                        class={"mr-4"}
                        style={{ width: 68, height: 68 }}
                        image={resources.IconPlaceholder}
                    />
                    <TextBox
                        class={"px-18"}
                        label={"To Join"} invert
                    />
                </div>
            </div>
        </div>
    );
}

function JoinScreen(props: ScreenProps) {
    const { menuState, navigate, game: { GameManager } } = props;

    const isLocal = menuState == MenuState.Local;
    const title = `${isLocal ? "Local" : "Online"} Co-Op`;

    return (
        <div class={"w-full h-full flex-col justify-between bg-boxgrad"}>
            <Label
                class={"py-8"}
                icon={resources.IconPlaceholder}
            >
                {title}
            </Label>

            <div class={"w-full flex-row justify-center mb-24"}>
                <PlayerCard index={0} class={"mr-[25%]"} />
                <PlayerCard index={1} />
            </div>

            <div class={"absolute w-full bottom-16"}>
                <div class={"self-center"}>
                    <TextBox
                        labelSize={Size.ExtraLarge}
                        label={"Start Game"}
                        onPress={() => {
                            if (isLocal) {
                                GameManager.StartLocalGame();
                            } else {
                                GameManager.StartRemoteGame();
                            }
                        }}
                        invert
                    />
                </div>
            </div>

            <div class={"flex-row self-end mr-16 pb-6"}>
                <TextBox
                    label={"Back"}
                    onPress={() => navigate("/play/coop")}
                    invert
                />
            </div>
        </div>
    );
}

export default JoinScreen;
