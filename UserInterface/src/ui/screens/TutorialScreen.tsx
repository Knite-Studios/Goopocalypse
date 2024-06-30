import { h } from "preact";

import { MenuState } from "@type/enums";

import { ScreenProps } from "@ui/App";

function TutorialScreen(props: ScreenProps) {
    const { menuState, navigate, game: { GameManager } } = props;

    const isLocal = menuState == MenuState.Local;

    return (
        <div>

        </div>
    );
}

export default TutorialScreen;
