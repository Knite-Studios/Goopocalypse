import { h } from "preact";

import Text, { Size } from "@components/Text";

import type { ScriptManager } from "game";

import { useEventfulState } from "onejs";
import resources from "@ui/resources";

/**
 * Format the score to a string.
 *
 * @param score The score to format.
 */
function formatScore(score: number): string {
    return score.toString().padStart(4, "0");
}

/**
 * Format the time to a string.
 * Time format: MM:SS
 *
 * @param time The time to format.
 */
function formatTime(time: number): string {
    const minutes = Math.floor(time / 60);
    const seconds = time % 60;

    return `${minutes.toString().padStart(2, "0")}:${seconds.toString().padStart(2, "0")}`;
}

function GameScreen({ game }: { game: ScriptManager }) {
    const { WaveManager } = game;

    const [score, _]: [number, any] = useEventfulState(
        WaveManager,
        "Score"
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
                    <Text size={Size.Large} class={"text-gray"}>
                        {formatTime(matchTimer)}
                    </Text>
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
                    <Text size={Size.Large} class={"text-gray"}>{formatScore(score)}</Text>
                </div>
            </div>
        </div>
    );
}

export default GameScreen;
