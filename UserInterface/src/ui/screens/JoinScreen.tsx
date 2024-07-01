import { h } from "preact";

import Card from "@components/Card";
import Label from "@components/LabelV2";

import { ScreenProps } from "@ui/App";

import { MenuState } from "@type/enums";

import resources, { gradient } from "@ui/resources";
import Button from "@components/ButtonV2";
import { useEventfulState } from "onejs";
import { PlayerSession } from "game";

function GradientBackground(props: { ready: boolean }) {
    return (
        <gradientrect
            vertical
            class={"w-full"}
            style={{ height: props.ready ? "100%" : "5%" }}
            colors={gradient}
        />
    );
}

function JoinScreen(props: ScreenProps) {
    const {
        navigate,
        lastPage,
        game: { GameManager, LobbyManager }
    } = props;

    const [players, _setPlayers] = useEventfulState(LobbyManager, "Players");

    const player1: PlayerSession | null = players[0] ?? null;
    const player2: PlayerSession | null = players[1] ?? null

    const allReady = player1?.isReady && player2?.isReady;

    return (
        <div class={"w-full h-full bg-dark-blue"}>
            <div
                class={
                    "absolute flex-row w-full h-full items-end justify-start"
                }
            >
                <GradientBackground ready={player1?.isReady ?? false} />
                <GradientBackground ready={player2?.isReady ?? false} />
            </div>

            <div class={"w-full h-full"}>
                <Label class={"mt-14 mb-[5%]"}>Online Co-Op</Label>

                <div class={"flex-row justify-center mb-[7%]"}>
                    <Card
                        class={"mr-[25%]"}
                        title={"Player 1"}
                        content={player1 && player1.isReady ? "Ready" : "Not Ready"}
                        picture={player1?.profileIcon ?? resources.Placeholder}
                    />

                    <Card
                        title={"Player 2"}
                        content={player2 ? player2.isReady ? "Ready" : "Not Ready" : "Press X to Invite"}
                        picture={player2?.profileIcon ?? resources.Placeholder}
                    />
                </div>

                <div class={"self-center w-[90%] flex-row justify-between"}>
                    <Button
                        disabled={!allReady}
                        class={`text-white ${allReady ? "" : "opacity-50"}`}
                        textClass={"px-24"}
                        onClick={() => GameManager.StartRemoteGame()}
                    >
                        Start
                    </Button>

                    <Button
                        bounce={false}
                        class={"text-white"}
                        textClass={"px-24"}
                        onClick={() => navigate("/")}
                    >
                        Back
                    </Button>
                </div>
            </div>
        </div>
    );
}

export default JoinScreen;
