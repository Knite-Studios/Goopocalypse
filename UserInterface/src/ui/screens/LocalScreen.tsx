import { h } from "preact";

import Label from "@components/LabelV2";

import { ScreenProps } from "@ui/App";

import { gradient } from "@ui/resources";
import Button from "@components/ButtonV2";
import Banner from "@components/Banner";
import Keyboard from "@components/Keyboard";
import { Size } from "@components/Text";

function JoinScreen(props: ScreenProps) {
    const {
        navigate,
        lastPage,
        game: { GameManager }
    } = props;

    return (
        <div class={"w-full h-full bg-dark-blue"}>
            <div
                class={
                    "absolute flex-row w-full h-full items-end justify-start"
                }
            >
                <gradientrect
                    vertical
                    class={"w-full h-full"}
                    colors={gradient}
                />
            </div>

            <div class={"w-full h-full"}>
                <Label class={"mt-14 mb-[8%]"}>Local Co-Op</Label>

                <div class={"flex-row justify-center mb-[9%]"}>
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
                </div>

                <div class={"self-center w-[90%] flex-row justify-between"}>
                    <Button
                        class={`text-white`}
                        textClass={"px-24"}
                        onClick={() => GameManager.StartLocalGame()}
                    >
                        Start
                    </Button>

                    <Button
                        bounce={false}
                        class={"text-white"}
                        textClass={"px-24"}
                        onClick={() => navigate(lastPage)}
                    >
                        Back
                    </Button>
                </div>
            </div>
        </div>
    );
}

export default JoinScreen;
