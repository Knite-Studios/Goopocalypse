import { h } from "preact";

import Button from "@components/ButtonV2";
import Image from "@components/Image";
import Text from "@components/Text";

import { ScreenProps } from "@ui/App";
import resources from "@ui/resources";

import { MenuState } from "@type/enums";

import { useEventfulState } from "onejs";

function MenuScreen({ game, navigate, setMenuState }: ScreenProps) {
    const { GameManager } = game;

    const [username, _] = useEventfulState(GameManager, "Username");
    const [pfp, __] = useEventfulState(GameManager, "ProfilePicture");

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
                            navigate("/join");
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
                    class={"text-white hover:text-green"}
                    onClick={() => navigate("/credits")}
                >
                    <Text>Credits</Text>
                </div>
            </div>
        </div>
    );
}

export default MenuScreen;
