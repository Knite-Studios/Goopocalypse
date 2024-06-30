import { h } from "preact";

import Banner from "@components/Banner";
import Button from "@components/ButtonV2";

import { ScreenProps } from "@ui/App";
import { gradient } from "@ui/resources";
import { Size } from "@components/Text";

function PauseScreen({ navigate }: ScreenProps) {
    return (
        <div class={"w-full h-full"}>
            <gradientrect
                vertical
                class={"absolute h-full left-40"}
                style={{ width: 470 }}
                // This gradient is actually flipped.
                colors={[gradient[1], gradient[0]]}
            >
                <Banner
                    class={"mt-32"}
                    textClass={"py-5"}
                    flagClass={"scale-150"}
                    size={Size.Normal}
                >
                    Game Paused
                </Banner>

                <div class={"mb-[50%]"} />

                <div class={"flex-col"}>
                    <Button
                        class={"mb-10"}
                        bounce={false}
                    >
                        Resume
                    </Button>

                    <Button
                        class={"mb-10"}
                        bounce={false}
                        onClick={() => navigate("/settings")}
                    >
                        Settings
                    </Button>

                    <Button
                        class={"mb-10"}
                        bounce={false}
                        onClick={() => navigate("/quit")}
                    >
                        Quit
                    </Button>
                </div>
            </gradientrect>
        </div>
    );
}

export default PauseScreen;
