// pause screen is responsible for both 'Pause Game' and 'Game Over'
// this is due to them being identical besides the title card
import { h } from "preact";

import Box from "@components/Box";
import { Size } from "@components/Text";
import TextBox from "@components/TextBox";

import { ScreenProps } from "@ui/App";

import { parseColor } from "onejs/utils/color-parser";

function PauseScreen({ game }: ScreenProps) {
    return (
        <div class={"flex-row w-full h-full"}>
            <gradientrect
                class={"left-1/4 absolute w-full h-full items-center"}
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

            <div
                class={"h-full mt-24 left-1/4 absolute"}
                style={{ width: 700, translate: "-50%" }}
            >
                <div class={"w-full h-full justify-between"}>
                    <Box>
                        <TextBox
                            positioning={{ rotate: -3.81 }}
                            class={"self-center"}
                            label={"Game Paused"}
                            labelSize={Size.Large}
                        />
                    </Box>

                    <div class={"flex-col"}>
                        <Box>
                            <TextBox label={"Resume"} labelSize={Size.Large} />
                        </Box>

                        <div class={"mb-6"} />

                        <Box>
                            <TextBox
                                label={"Settings"}
                                labelSize={Size.Large}
                            />
                        </Box>

                        <div class={"mb-6"} />

                        <Box>
                            <TextBox
                                label={"Quit Game"}
                                labelSize={Size.Large}
                            />
                        </Box>
                    </div>

                    <div />
                </div>
            </div>
        </div>
    );
}

export default PauseScreen;
