import { h } from "preact";
import { Style } from "preact/jsx";

import Text, { Size } from "@components/Text";

import { Texture2D } from "UnityEngine";
import Shadow from "@components/Shadow";

interface IProps {
    label: string;
    labelSize?: Size;

    icon?: Texture2D;
    class?: string;

    buttonClass?: string;
    buttonStyle?: Style;
    onPress?: () => void; // Having this declared makes this element a button.
}

function TextBox(props: IProps) {
    const isButton = props.onPress !== undefined;

    const box = <div class={"flex flex-row-reverse"}>
        <Shadow color={"#bfbfbf"} radius={5}>
            <div class={props.class +
                " grow-0 w-auto flex-row py-2.5 px-[20px] bg-boxgrad text-boxtext items-center"}
                 style={{ minWidth: "auto", maxWidth: "none" }}
            >
                { props.icon && (
                    <image
                        class={"w-11 h-11 mr-[20px]"}
                        image={props.icon}
                    />
                ) }

                <Text
                    size={props.labelSize || Size.Normal}
                >
                    {props.label}
                </Text>
            </div>
        </Shadow>

        <div class={"flex-row w-full hidden"}>a</div>
    </div>;

    return isButton ? (
        <button
            class={props.buttonClass}
            style={props.buttonStyle}
            onClick={props.onPress}
        >
            {box}
        </button>
    ) : box;
}

export default TextBox;
