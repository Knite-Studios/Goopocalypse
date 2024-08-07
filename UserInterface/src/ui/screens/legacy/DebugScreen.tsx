import { h } from "preact";
import { useState } from "preact/hooks";

import Button from "@components/Button";
import Text, { Size } from "@components/Text";

import type { ScriptManager } from "game";
import router from "@ui/Router";
import { ScreenProps } from "@ui/App";

function DebugScreen({ game, navigate }: ScreenProps) {
    const [address, setAddress] = useState("127.0.0.1");
    const [port, setPort] = useState(7777);

    return (
        <div style={{ width: "50%" }} class={"p-5 text-white"}>
            <Text size={Size.Normal} class={"mb-4"}>
                Hello World!
            </Text>

            <Button
                class={"mb-4 bg-blue-500 "}
                onClick={() => {
                    game.GameManager.StartDebugServer();
                    game.GameManager.StartRemoteGame();
                }}
            >
                Start Game (host + server)
            </Button>

            <div class={"flex-row w-full mb-4"}>
                <textfield
                    text={"127.0.0.1"}
                    class={"text-black w-1/2 text-2xl"}
                    onInput={(e) => setAddress(e.newData)}
                />

                <textfield
                    text={"7777"}
                    class={"text-black w-1/2 text-2xl"}
                    onInput={(e) => setPort(parseInt(e.newData))}
                />
            </div>

            <Button
                class={"mb-4 bg-blue-500"}
                onClick={() => {
                    game.GameManager.JoinDebugServer(address, port);
                    game.GameManager.StartRemoteGame();
                }}
            >
                Join Game (client)
            </Button>

            <Button
                class={"mb-4 bg-blue-500"}
                onClick={() => {
                    navigate("/debug/lobby");
                    game.LobbyManager.MakeLobby();
                }}
            >
                Create Steam Lobby
            </Button>

            <Button
                class={"mb-4 bg-blue-500"}
                onClick={() => {
                    game.GameManager.StartLocalGame();
                }}
            >
                Start Local Game
            </Button>
        </div>
    );
}

export default DebugScreen;
