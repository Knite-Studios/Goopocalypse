import { h } from "preact";

import Label from "@components/Label";
import { Size } from "@components/Text";
import TextBox from "@components/TextBox";

import * as resources from "@ui/resources";

import type { ScriptManager } from "game";

function SoloScreen({ game }: { game: ScriptManager }) {
    return (
        <div class={"py-8 w-full h-full bg-white"}>
            <Label
                class={"mb-12"}
                icon={resources.IconPlaceholder}
            >
                Solo
            </Label>

            <div class={"flex-row justify-center"}>
                <div class={"flex-col justify-between"}>
                    <image
                        class={"w-[80%]"}
                        image={resources.Placeholder}
                    />

                    <image
                        class={"w-[80%]"}
                        image={resources.Placeholder}
                    />
                </div>

                <image
                    image={resources.VerticalPlaceholder}
                />
            </div>

            <div class={"flex-row-reverse"}>
                <div />

                <TextBox
                    labelSize={Size.Large}
                    label={"Press Any Button"}
                    positioning={{
                        position: "Absolute",
                        bottom: 15, right: 50,
                        rotate: -3
                    }}
                />
            </div>
        </div>
    );
}

export default SoloScreen;
