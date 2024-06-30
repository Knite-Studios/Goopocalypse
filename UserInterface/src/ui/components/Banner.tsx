import { h } from "preact";
import { Style } from "preact/jsx";

import Text, { Size } from "@components/Text";

import resources from "@ui/resources";
import { Texture2D } from "UnityEngine";

interface IProps {
    class?: string;
    textClass?: string;
    flagClass?: string;

    minWidth?: number;
    background?: Texture2D;

    style?: Style;
    size?: Size;

    children: string;
}

export function MiniFlag({ rotate, class: flagClass }: { class?: string, rotate?: boolean }) {
    const invert = rotate ? 1 : -1;

    return (
        <image
            class={`self-end mb-5 ${flagClass || ""}`}
            image={resources.MiniFlag}
            style={{
                rotate: rotate ? 180 : 0,
                width: 54,
                height: 45,

                right: 35 * invert,
                top: 30
            }}
        />
    );
}

function Banner(props: IProps) {
    return (
        <div
            class={`flex-row ${props.class || ""}`}
            style={props.style}
        >
            <div class={"flex-row justify-between w-full h-full absolute"}>
                <MiniFlag class={props.flagClass} rotate />
                <MiniFlag class={props.flagClass} />
            </div>

            <div
                class={"w-full flex-col items-center justify-center"}
                style={{
                    minWidth: props.minWidth ?? 360,
                    minHeight: 70,
                    backgroundImage: props.background ?? resources.ButtonBackground
                }}
            >
                <Text
                    size={props.size || Size.Small}
                    class={`text-white ${props.textClass || ""}`}
                >
                    {props.children}
                </Text>
            </div>
        </div>
    );
}

export default Banner;
