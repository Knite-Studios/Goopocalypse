import { h } from "preact";
import { useState } from "preact/hooks";
import { Style } from "preact/jsx";

import GifRenderer from "@components/GifRenderer";
import Text, { Size } from "@components/Text";

import resources from "@ui/resources";
import { ScriptManager } from "game";

const { AudioManager } = require("game") as ScriptManager;

interface IProps {
    // Container properties.
    style?: Style;
    class?: string;

    // Text properties.
    textClass?: string;
    size?: Size;
    children: string;
    defaultColor?: string;

    // Button properties.
    bounce?: boolean;
    disabled?: boolean;

    onClick?: () => void;
}

function Button(props: IProps) {
    const disabled = props.disabled;
    const bounce = props.bounce ?? true;

    const [hover, setHover] = useState(false);

    return (
        <button
            class={`${
                props.class ?? ""
            } px-16 py-6 border-none border-0 bg-transparent`}
            style={{
                ...props.style,
                backgroundImage: hover && resources.ButtonBackground
            }}
            onMouseOver={() => {
                if (disabled) return;

                setHover(true);
                AudioManager.PlayUIHoverSound();
            }}
            onMouseOut={() => setHover(false)}
            onClick={() => {
                if (disabled) return;

                props.onClick?.();
                AudioManager.PlayUIClickSound();
            }}
        >
            {bounce && hover && (
                <GifRenderer
                    start={1}
                    end={12}
                    fps={12}
                    frames={"fwendgif"}
                    prefix={"Fwend"}
                    style={{
                        position: "Absolute",
                        left: 0,
                        bottom: 36
                    }}
                />
            )}

            <Text
                class={`text ${props.textClass || ""}`}
                style={{
                    translate: [bounce && hover ? 42 : 0, 0],
                    color: hover ? "#72e8e6" : props.defaultColor || "white"
                }}
                size={props.size ?? Size.Normal}
            >
                {props.children}
            </Text>
        </button>
    );
}

export default Button;
