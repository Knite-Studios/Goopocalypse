import { h } from "preact";
import usePrevious from "@app/hooks/usePrevious";

import CreditsScreen from "@screens/CreditsScreen";
import JoinScreen from "@screens/JoinScreen";
import MenuScreen from "@screens/MenuScreen";
import TutorialScreen from "@screens/TutorialScreen";
// TODO: Remove legacy code.
import DebugScreen from "@screens/legacy/DebugScreen";
import LocalScreen from "@screens/LocalScreen";
import GameScreen from "@screens/overlays/GameScreen";
import PauseScreen from "@screens/overlays/PauseScreen";
import QuitScreen from "@screens/overlays/QuitScreen";
import WaveScreen from "@screens/overlays/WaveScreen";
import FinishScreen from "@screens/overlays/FinishScreen";
import SettingsScreen from "@screens/overlays/SettingsScreen";

import Router, { Route } from "@ui/Router";

import { MenuState } from "@type/enums";

import { ScriptManager } from "game";

import { useEventfulState } from "onejs";

const game = require("game") as ScriptManager;

export type ScreenProps = {
    game: ScriptManager;
    menuState: MenuState;

    lastPage: string;
    setMenuState: (state: MenuState) => void;
    navigate: (r: string) => void;
};

function App() {
    const { GameManager } = game;

    const [currentRoute, navigate] = useEventfulState(GameManager, "Route");
    const previousPage = usePrevious(currentRoute);

    const [gameState, _] = useEventfulState(GameManager, "State");

    return (
        <Router
            game={game}
            setRoute={navigate}
            route={currentRoute}
            previous={previousPage}
        >
            <Route path={"/"} element={MenuScreen} />
            <Route path={"/debug"} element={DebugScreen} />

            <Route path={"/join"} element={JoinScreen} />
            <Route path={"/join/local"} element={LocalScreen} />
            <Route path={"/credits"} element={CreditsScreen} />
            <Route path={"/tutorial"} element={TutorialScreen} />

            {/* This can also be an overlay. */}
            <Route path={"/quit"} element={QuitScreen} />
            <Route path={"/settings"} element={SettingsScreen} />

            <Route path={"/game"} element={GameScreen} />
            <Route path={"/game/over"} element={FinishScreen} />
            <Route path={"/game/wave"} element={WaveScreen} />
            <Route path={"/game/pause"} element={PauseScreen} />
        </Router>
    );
}

export default App;
