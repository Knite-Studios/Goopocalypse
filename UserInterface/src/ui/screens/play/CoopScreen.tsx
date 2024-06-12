import { h } from "preact";

import Label from "@components/Label";

import * as resources from "@ui/resources";

import type { ScriptManager } from "game";

import { parseColor } from "onejs/utils/color-parser";
import TextBox from "@components/TextBox";
import { Size } from "@components/Text";

function CoopScreen({ game }: { game: ScriptManager }) {
    return (
        <div class={"w-full h-full bg-boxgrad"}>
            <gradientrect
                class={"absolute w-full left-1/2"}
                style={{
                    height: 470,
                    translate: "-50%",
                    rotate: 90
                }}
                colors={[
                    parseColor("rgba(204, 204, 255, 1)"),
                    parseColor("rgba(204, 204, 255, 0)")
                ]}
            />

            <div class={"py-8 w-full h-full flex-col justify-between"}>
                <Label
                    icon={resources.IconPlaceholder}
                >
                    Co-Op
                </Label>

                <div class={"ml-[128px] w-full flex-row justify-between items-center"}>
                    <div>
                        <TextBox labelSize={Size.ExtraLarge} label={"Local Co-Op"} />
                        <div class={"mb-14"} />
                        <div class={"flex-row"}>
                            <TextBox labelSize={Size.Large} label={"Online Co-Op"} invert />
                        </div>
                    </div>

                    <image
                        image={resources.Placeholder}
                    />

                    <div class={"mr-14"} />
                </div>

                <div class={"flex-row self-end mr-16"}>
                    <TextBox label={"Select"} invert />
                    <div class={"mr-6"} />
                    <TextBox label={"Back"} invert />
                </div>
            </div>
        </div>
    );
}

export default CoopScreen;
