import { h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { useEventfulState } from "onejs";

import Router, { Route } from "@ui/Router";

import DebugScreen from "@screens/DebugScreen";
import MenuScreen from "@screens/MenuScreen";
import LobbyScreen from "@screens/LobbyScreen";
import GameScreen from "@screens/GameScreen";

import CoopScreen from "@screens/play/CoopScreen";

import { ScriptManager } from "game";
import { GameState } from "@type/enums";
import SoloScreen from "@screens/play/SoloScreen";

const game = require("game") as ScriptManager;

export type ScreenProps = {
    game: ScriptManager,
    navigate: (r: string) => void;
};

function App() {
    const { GameManager } = game;

    const [currentRoute, navigate] = useState(GameManager.DefaultRoute);
    const [gameState, setGameState] = useEventfulState(GameManager, "State");

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
                <Router setRoute={navigate} route={currentRoute}>
                    <Route path={"/"} element={<DebugScreen game={game} />} />
                    <Route path={"/play/solo"} element={<SoloScreen game={game} />} />
                    <Route path={"/play/coop"} element={<CoopScreen game={game} />} />
                    <Route path={"/debug"} element={<DebugScreen game={game} />} />
                </Router>
            );
        case GameState.Lobby:
            return <LobbyScreen game={game} />;
        case GameState.Playing:
            return <GameScreen game={game} />;
        default:
            return <div></div>;
    }
}

export default App;
