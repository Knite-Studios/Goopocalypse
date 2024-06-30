import { h } from "preact";
import { useEffect, useState } from "preact/hooks";

import { useEventfulState } from "onejs";

import Router, { Route } from "@ui/Router";

import DebugScreen from "@screens/DebugScreen";
import MenuScreen from "@screens/MenuScreen";
import LobbyScreen from "@screens/LobbyScreen";
import GameScreen from "@screens/GameScreen";

import JoinScreen from "@screens/JoinScreenV2";

import PauseScreen from "@screens/overlays/PauseScreen";

import { ScriptManager } from "game";
import { GameState, MenuState } from "@type/enums";

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
                <Router game={game} setRoute={navigate} route={currentRoute}>
                    <Route path={"/"} element={MenuScreen} />
                    <Route path={"/join"} element={JoinScreen} />
                    <Route path={"/game/pause"} element={PauseScreen} />
                    <Route path={"/debug"} element={DebugScreen} />
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
