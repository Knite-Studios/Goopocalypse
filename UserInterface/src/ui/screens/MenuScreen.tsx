import { h } from "preact";
import { useState } from "preact/hooks";

import Button from "@components/ButtonV2";
import Image from "@components/Image";
import Text from "@components/Text";

import { ScreenProps } from "@ui/App";
import resources from "@ui/resources";

import { MenuState } from "@type/enums";

import { useEventfulState } from "onejs";
import GifRenderer from "@components/GifRenderer";
import { ScriptManager } from "game";

const { AudioManager } = require("game") as ScriptManager;

function MenuScreen({ game, navigate, setMenuState }: ScreenProps) {
    const { GameManager } = game;

    const [username, _] = useEventfulState(GameManager, "Username");
    const [pfp, __] = useEventfulState(GameManager, "ProfilePicture");

    const [hovered, setHovered] = useState(false);

    return (
        <div class={"w-full h-full flex-row"}>
            <div
                class={
                    "w-[40%] h-full flex-col items-center justify-between bg-dark-blue"
                }
            >
                <Image
                    src={resources.Logo}
                    class={"self-center mt-12"}
                    style={{ maxWidth: 318, maxHeight: 253 }}
                />

                <div class={"flex-col justify-between"}>
                    <Button
                        class={"mb-8"}
                        onClick={() => {
                            setMenuState(MenuState.Local);
                            navigate("/join/local");
                        }}
                    >
                        Play Local
                    </Button>

                    <Button
                        class={"mb-8"}
                        onClick={() => {
                            setMenuState(MenuState.Online);
                            navigate("/join");
                        }}
                    >
                        Play Online
                    </Button>

                    <Button
                        class={"mb-8"}
                        onClick={() => navigate("/settings")}
                    >
                        Settings
                    </Button>

                    <Button
                        class={"mb-16"}
                        onClick={() => navigate("/quit")}
                    >
                        Quit Game
                    </Button>
                </div>
            </div>

            <div class={"px-14 py-6 w-full flex-col justify-between items-end"}>
                <div
                    class={"flex-row items-center px-8 py-2"}
                    style={{
                        backgroundImage: resources.ProfileFrame
                    }}
                >
                    <Image
                        class={"mr-5"}
                        style={{ width: 42, height: 42 }}
                        src={pfp}
                    />
                    <Text class={"text-green"}>{username}</Text>
                </div>

                <div
                    class={"items-center text-white hover:text-green"}
                    onClick={() => {
                        navigate("/credits");
                        AudioManager.PlayUIClickSound();
                    }}
                    onMouseOver={() => {
                        setHovered(true);
                        AudioManager.PlayUIHoverSound();
                    }}
                    onMouseOut={() => setHovered(false)}
                >
                    { hovered && <GifRenderer start={1} end={12} frames={"fwendgif"}
                                              prefix={"Fwend"} fps={12} style={{ right: 12 }} /> }

                    <Text>Credits</Text>
                </div>
            </div>
        </div>
    );
}

export default MenuScreen;
