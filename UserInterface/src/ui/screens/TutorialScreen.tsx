import { h } from "preact";

import Text, { Size } from "@components/Text";
import Label from "@components/LabelV2";
import Banner from "@components/Banner";

import { ScreenProps } from "@ui/App";

import { MenuState } from "@type/enums";

import resources, { gradient } from "@ui/resources";
import Image from "@components/Image";

function Key(props: { children: string, class?: string }) {
    return (
        <div
            class={props.class}
            style={{
                backgroundImage: resources.KeyCap,
                width: 126, height: 136
            }}
        >
            <Text
                size={Size.ExtraLarge}
                style={{ left: 5 }}
                class={"text-white text-center self-center"}
            >
                {props.children}
            </Text>
        </div>
    );
}

function Keyboard({ keys }: { keys: string[] }) {
    if (keys.length != 4) return <div />;

    return (
        <div class={"flex-col items-center"}>
            <Key class={"mb-6"}>{keys[0]}</Key>

            <div class={"flex-row"}>
                <Key class={"mr-10"}>{keys[1]}</Key>
                <Key class={"mt-2 mr-10"}>{keys[2]}</Key>
                <Key>{keys[3]}</Key>
            </div>
        </div>
    );
}

function TutorialScreen(props: ScreenProps) {
    const { menuState } = props;

    const isLocal = menuState == MenuState.Local;

    return (
        <div class={"w-full h-full bg-dark-blue"}>
            <gradientrect
                vertical
                class={"absolute bottom-0 w-full h-[50%]"}
                colors={gradient}
            />

            <div class={"w-full h-full"}>
                <Label class={"mt-14 mb-[10%]"}>Controls</Label>

                <div class={"flex-row justify-center"}>
                    <div class={"flex-col mr-[15%]"}>
                        <Keyboard keys={["W", "A", "S", "D"]} />

                        <div class={"mb-8"} />

                        <Banner
                            size={Size.Normal}
                            textClass={"py-5"}
                            class={"w-[550px]"}
                        >
                            Player 1
                        </Banner>
                    </div>

                    {
                        isLocal ? (
                            <div class={"flex-col"}>
                                <Keyboard keys={["I", "J", "K", "L"]} />

                                <div class={"mb-8"} />

                                <Banner
                                    size={Size.Normal}
                                    textClass={"py-5"}
                                    class={"w-[550px]"}
                                >
                                    Player 2
                                </Banner>
                            </div>
                        ) : (
                            <div>
                                <Image
                                    src={resources.Tutorial}
                                    class={"top-10"}
                                    style={{ maxWidth: 500, maxHeight: 350 }}
                                />

                                <Banner
                                    size={Size.Normal}
                                    textClass={"py-5"}
                                    class={"w-[550px] absolute bottom-0 left-1/2"}
                                    style={{ translate: "-50%" }}
                                >
                                    Kill the Slimes
                                </Banner>
                            </div>
                        )
                    }
                </div>
            </div>
        </div>
    );
}

export default TutorialScreen;
