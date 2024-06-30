import { h } from "preact";

import { ScreenProps } from "@ui/App";

import { MenuState } from "@type/enums";

function TutorialScreen(props: ScreenProps) {
    const {
        menuState,
        navigate,
        game: { GameManager }
    } = props;

    const isLocal = menuState == MenuState.Local;

    return <div></div>;
}

export default TutorialScreen;
