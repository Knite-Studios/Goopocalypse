import { h } from "preact";

import Card from "@components/Card";
import Label from "@components/LabelV2";
import Text from "@components/Text";

import { ScreenProps } from "@ui/App";

import { MenuState } from "@type/enums";

import { gradient } from "@ui/resources";

function GradientBackground(props: { ready: boolean }) {
    return (
        <gradientrect
            vertical
            class={"w-full"}
            style={{ height: props.ready ? "100%" : "5%" }}
            colors={gradient}
        />
    );
}

function JoinScreen(props: ScreenProps) {
    const {
        menuState,
        navigate,
        game: { GameManager }
    } = props;

    const isLocal = menuState == MenuState.Local;
    const title = `${isLocal ? "Local" : "Online"} Co-Op`;

    const allReady = false;

    return (
        <div class={"w-full h-full bg-dark-blue"}>
            <div
                class={
                    "absolute flex-row w-full h-full items-end justify-start"
                }
            >
                <GradientBackground ready={false} />
                <GradientBackground ready={true} />
            </div>

            <div class={"w-full h-full"}>
                <Label class={"mt-14 mb-[5%]"}>{title}</Label>

                <div class={"flex-row justify-center mb-[8%]"}>
                    <Card
                        class={"mr-[25%]"}
                        title={"Player 1"}
                        content={"Not Ready"}
                        picture={GameManager.ProfilePicture}
                    />

                    <Card
                        title={"Player 2"}
                        content={"Ready"}
                        picture={GameManager.ProfilePicture}
                    />
                </div>

                <div class={"self-center w-[75%] flex-row justify-between"}>
                    <Text class={`text-white ${allReady ? "" : "opacity-50"}`}>
                        Start
                    </Text>

                    <Text class={"text-white"}>Back</Text>
                </div>
            </div>
        </div>
    );
}

export default JoinScreen;
