import { h } from "preact";
import { useState } from "preact/hooks";

import Text from "@components/Text";
import TextBox from "@components/TextBox";

import type { ScreenProps } from "@ui/App";
import * as resources from "@ui/resources";

import { useEventfulState } from "onejs";

interface IMenuButtonProps {
    bottom?: boolean;
    onClick: () => void;

    children: string;
}

function MenuButton(props: IMenuButtonProps) {
    const { children, onClick } = props;

    const [highlighted, setHighlighted] = useState(false);

    return (
        <div class={"flex-col"}>
            { highlighted ? (
                <div class={"flex-row"}>
                    <TextBox
                        onMouseOver={() => setHighlighted(true)}
                        onMouseOut={() => setHighlighted(false)}
                        label={children}
                        onPress={onClick}
                    />

                    <div />
                </div>
            ) : (
                <div
                    onClick={onClick}
                    onMouseOver={() => setHighlighted(true)}
                    onMouseOut={() => setHighlighted(false)}
                >
                    <Text class={"text-boxtext"}>
                        {children}
                    </Text>
                </div>
            ) }

            { !props.bottom && <div class={"mb-7 invisible"} /> }
        </div>
    );
}

function MenuScreen({ game, navigate }: ScreenProps) {
    const { GameManager } = game;

    const [username, _] = useEventfulState(GameManager, "Username");
    const [pfp, __] = useEventfulState(GameManager, "ProfilePicture");

    return (
        <div class={"bg-white w-full h-full flex-row justify-between p-7"}>
            <div>
                <image
                    class={"mb-16"}
                    image={resources.Logo}
                />

                <div class={"flex-col ml-16"}>
                    <MenuButton onClick={() => navigate("/play/solo")}>
                        Play Solo
                    </MenuButton>

                    <MenuButton onClick={() => navigate("/play/coop")}>
                        Play Co-Op
                    </MenuButton>

                    <MenuButton onClick={() => null}>
                        Store
                    </MenuButton>

                    <MenuButton onClick={() => null}>
                        How to Play
                    </MenuButton>

                    <MenuButton onClick={() => null}>
                        Settings
                    </MenuButton>

                    <MenuButton
                        bottom
                        onClick={() => GameManager.QuitGame()}
                    >
                        Quit Game
                    </MenuButton>
                </div>
            </div>

            <div class={"flex-col justify-between"}>
                <TextBox
                    containerClass={"flex-row-reverse"}
                    icon={pfp}
                    label={username}
                />

                <div class={"self-end flex-row mr-[5%]"}>
                    <TextBox
                        label={"Credits"}
                    />

                    {/* This is used as padding to ensure shadows work. */}
                    <div class={"mr-7 invisible"} />

                    <TextBox
                        label={"Select"}
                    />
                </div>
            </div>
        </div>
    );
}

export default MenuScreen;
