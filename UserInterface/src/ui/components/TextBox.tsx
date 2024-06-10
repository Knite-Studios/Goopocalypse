import { h } from "preact";

import Shadow from "@components/Shadow";
import Text, { Size } from "@components/Text";

import { Texture2D } from "UnityEngine";

interface IProps {
    label: string;
    labelSize?: Size;

    icon?: Texture2D;
    class?: string;
    containerClass?: string;

    onPress?: () => void; // Having this declared makes this element a button.
    onMouseOver?: () => void;
    onMouseOut?: () => void;
}

function TextBox(props: IProps) {
    return (
        <div
            class={props.containerClass + " flex"}
            onClick={props.onPress}
            onMouseOut={props.onMouseOut}
        >
            <Shadow color={"#bfbfbf"} radius={5}>
                <div class={"grow-0 w-auto flex-row py-2.5 px-[20px] " +
                    "bg-boxgrad text-boxtext items-center " +
                    props.class}
                     style={{ minWidth: "auto", maxWidth: "none" }}
                     onMouseOver={props.onMouseOver}
                     onMouseOut={props.onMouseOut}
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
        </div>
    );
}

export default TextBox;
