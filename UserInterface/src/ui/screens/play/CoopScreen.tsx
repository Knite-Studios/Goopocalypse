import { h } from "preact";

import Label from "@components/Label";

import * as resources from "@ui/resources";

import type { ScriptManager } from "game";

import { parseColor } from "onejs/utils/color-parser";
import TextBox from "@components/TextBox";

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

                <div class={"w-full flex-row justify-between items-center"}>
                    <div>
                        <TextBox label={"Local Co-Op"} />
                        <TextBox label={"Online Co-Op"} />
                    </div>

                    <image
                        image={resources.Placeholder}
                    />
                </div>

                <div class={"flex-row self-end"}>
                    <TextBox label={"Select"} />
                    <div class={"mr-6"} />
                    <TextBox label={"Back"} />
                </div>
            </div>
        </div>
    );
}

export default CoopScreen;
