import { h } from "preact";

import { useEventfulState } from "onejs";

import DebugScreen from "./screens/DebugScreen";

import MenuScreen from "@screens/MenuScreen";
import LobbyScreen from "@screens/LobbyScreen";
import GameScreen from "@screens/GameScreen";

import { ScriptManager } from "game";
import { GameState } from "@type/enums";

const game = require("game") as ScriptManager;

function App() {
    const [gameState, setGameState] = useEventfulState(game.GameManager, "State");

    switch (gameState) {
        case GameState.Menu:
            return <DebugScreen game={game} />;
        case GameState.Lobby:
            return <LobbyScreen game={game} />;
        case GameState.Playing:
            return <GameScreen game={game} />;
        default:
            return <div></div>;
    }
}

export default App;
