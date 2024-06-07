import { h } from "preact";

import TextBox from "@components/TextBox";

import * as resources from "@ui/resources";

import type { ScriptManager } from "game";
import { useEventfulState } from "onejs";

interface IMenuButtonProps {
    highlighted: boolean;
    onClick: () => void;

    children: string;
}

function MenuButton(props: IMenuButtonProps) {
    const { highlighted, children, onClick } = props;

    return highlighted ? (
        <TextBox
            label={children}
            onPress={onClick}
        />
    ) : (
        <div
            onClick={onClick}
        >
            {children}
        </div>
    );
}

function MenuScreen({ game }: { game: ScriptManager }) {
    const { GameManager } = game;

    const [username, _] = useEventfulState(GameManager, "Username");
    const [pfp, __] = useEventfulState(GameManager, "ProfilePicture");

    return (
        <div class={"w-full h-full flex-row justify-between p-7 bg-white"}>
            <div>
                <image
                    image={resources.Logo}
                />
            </div>

            <div class={"flex-col justify-between"}>
                <TextBox
                    icon={pfp}
                    label={username}
                />

                <image
                    class={"mr-[15%]"}
                    image={resources.Placeholder}
                />

                <div class={"self-end flex-row mr-[5%]"}>
                    <TextBox
                        label={"Credits"}
                    />

                    {/* This is used as padding to ensure shadows work. */}
                    <div class={"mr-7 invisible"}>a</div>

                    <TextBox
                        label={"Select"}
                    />
                </div>
            </div>
        </div>
    );
}

export default MenuScreen;
