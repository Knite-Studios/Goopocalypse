import { h } from "preact";

import Text, { Size } from "@components/Text";

import type { ScriptManager } from "game";

import { useEventfulState } from "onejs";
import resources, { BackwardsScoreCard } from "@ui/resources";
import Image from "@components/Image";

function GameScreen({ game }: { game: ScriptManager }) {
    const { WaveManager } = game;

    const [waveCount, _]: [number, any] = useEventfulState(
        WaveManager,
        "WaveCount"
    );
    const [matchTimer, __]: [number, any] = useEventfulState(
        WaveManager,
        "MatchTimer"
    );

    return (
        <div class={"flex-row w-full justify-between mt-4"}>
            <div class={"flex-col"}>
                <div
                    class={"mb-6 items-center justify-center"}
                    style={{
                        backgroundImage: resources.FullFlag,
                        width: 277, height: 94
                    }}
                >
                    <Text class={"text-white mr-8"}>
                        Time
                    </Text>
                </div>

                <div
                    class={"w-full h-full justify-center items-center"}
                    style={{
                        backgroundImage: resources.BackwardsScoreCard,
                        width: 382, height: 111
                    }}
                >
                    <Text size={Size.Large} class={"text-gray"}>00:00</Text>
                </div>
            </div>

            <div class={"flex-col items-end"}>
                <div
                    class={"mb-6 items-end justify-center"}
                    style={{
                        backgroundImage: resources.BackwardsFlag,
                        width: 277, height: 94
                    }}
                >
                    <Text class={"text-white mr-8"}>
                        Score
                    </Text>
                </div>

                <div
                    class={"w-full h-full justify-center items-center"}
                    style={{
                        backgroundImage: resources.ScoreCard,
                        width: 382, height: 111
                    }}
                >
                    <Text size={Size.Large} class={"text-gray"}>xxxx</Text>
                </div>
            </div>
        </div>
    );
}

export default GameScreen;
