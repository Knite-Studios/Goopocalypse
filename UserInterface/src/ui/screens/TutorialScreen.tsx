import { h } from "preact";

import { Size } from "@components/Text";
import Label from "@components/LabelV2";
import Banner from "@components/Banner";
import Image from "@components/Image";
import Keyboard from "@components/Keyboard";

import { ScreenProps } from "@ui/App";

import resources, { gradient } from "@ui/resources";

function TutorialScreen(_: ScreenProps) {
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
                            Move
                        </Banner>
                    </div>

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
                </div>
            </div>
        </div>
    );
}

export default TutorialScreen;
