import { h } from "preact";
import { useEffect, useState } from "preact/hooks";

import CreditsScreen from "@screens/CreditsScreen";
import JoinScreen from "@screens/JoinScreen";
import MenuScreen from "@screens/MenuScreen";
import TutorialScreen from "@screens/TutorialScreen";
// TODO: Remove legacy code.
import DebugScreen from "@screens/legacy/DebugScreen";
import LobbyScreen from "@screens/legacy/LobbyScreen";
import GameScreen from "@screens/overlays/GameScreen";
import PauseScreen from "@screens/overlays/PauseScreen";
import QuitScreen from "@screens/overlays/QuitScreen";
import WaveScreen from "@screens/overlays/WaveScreen";
import FinishScreen from "@screens/overlays/FinishScreen";

import Router, { Route } from "@ui/Router";

import { GameState, MenuState } from "@type/enums";

import { ScriptManager } from "game";

import { useEventfulState } from "onejs";

const game = require("game") as ScriptManager;

export type ScreenProps = {
    game: ScriptManager;
    menuState: MenuState;

    setMenuState: (state: MenuState) => void;
    navigate: (r: string) => void;
};

function App() {
    const { GameManager } = game;

    const [currentRoute, navigate] = useState(GameManager.DefaultRoute);
    const [gameState, _] = useEventfulState(GameManager, "State");

    // Register the router event listener.
    useEffect(() => {
        GameManager.remove_OnRouteUpdate(navigate);
        GameManager.add_OnRouteUpdate(navigate);

        onEngineReload(() => {
            GameManager.remove_OnRouteUpdate(navigate);
        });
    }, []);

    switch (gameState) {
        case GameState.Menu:
            return (
                <Router game={game} setRoute={navigate} route={currentRoute}>
                    <Route path={"/"} element={MenuScreen} />
                    <Route path={"/debug"} element={DebugScreen} />

                    <Route path={"/join"} element={JoinScreen} />
                    <Route path={"/credits"} element={CreditsScreen} />
                    <Route path={"/tutorial"} element={TutorialScreen} />

                    {/* This can also be an overlay. */}
                    <Route path={"/quit"} element={QuitScreen} />
                    <Route path={"/settings"} element={PauseScreen} />

                    <Route path={"/game"} element={PauseScreen} />
                    <Route path={"/game/over"} element={FinishScreen} />
                    <Route path={"/game/wave"} element={WaveScreen} />
                    <Route path={"/game/pause"} element={PauseScreen} />
                </Router>
            );
        case GameState.Lobby:
            // TODO: Remove legacy code.
            return <LobbyScreen game={game} />;
        case GameState.Playing:
            return <GameScreen game={game} />;
        default:
            return <div></div>;
    }
}

export default App;
