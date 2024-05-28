import { h } from "preact";

import { useEventfulState } from "onejs";

import Text, { Size } from "@components/Text";

import type { ScriptManager } from "game";

function GameScreen({ game }: { game: ScriptManager }) {
    const { WaveManager } = game;

    const [waveCount, _]: [number, any] = useEventfulState(WaveManager, "WaveCount");
    const [matchTimer, __]: [number, any] = useEventfulState(WaveManager, "MatchTimer");

    return (
        <div class={"p-16 flex flex-col w-full h-full text-blue-400"}>
            <div class={"flex flex-col self-center text-center"}>
                <Text size={Size.Normal}>{`Wave ${waveCount.toString()}`}</Text>
                <Text size={Size.Large}>{matchTimer.toString()}</Text>
            </div>
        </div>
    );
}

export default GameScreen;
