import { h } from "preact";

import { ScreenProps } from "@ui/App";
import resources, { gradient } from "@ui/resources";
import Banner from "@components/Banner";
import Text, { Size } from "@components/Text";
import Button from "@components/ButtonV2";

function FinishScreen({ game, navigate }: ScreenProps) {
    return (
        <div class={"flex-row justify-between w-full h-full"}>
            <gradientrect
                vertical
                class={"h-full left-40"}
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
                    Game Over
                </Banner>

                <div class={"mb-[50%]"} />

                <div class={"flex-col"}>
                    <Button
                        class={"mb-10"}
                        bounce={false}
                    >
                        Restart
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
                    >
                        Quit
                    </Button>
                </div>
            </gradientrect>

            <div class={"w-full h-full right-0 items-end justify-center"}>
                <div
                    class={"w-full h-full flex-row items-center justify-center"}
                    style={{
                        backgroundImage: resources.BackwardsFlag,
                        maxWidth: 604, maxHeight: 205
                    }}
                >
                    <Text size={Size.ExtraLarge} class={"text-white ml-24"}>Score</Text>
                </div>

                <div class={"mb-8"} />

                <div
                    class={"w-full h-full justify-center items-center"}
                    style={{
                        backgroundImage: resources.ScoreCard,
                        maxWidth: 824, maxHeight: 241
                    }}
                >
                    <Text size={Size.ExtraLarge} class={"text-gray"}>xxxx</Text>
                </div>
            </div>
        </div>
    );
}

export default FinishScreen;
