import { h } from "preact";
import { Style } from "preact/jsx";
import { useState } from "preact/hooks";

import Text, { Size } from "@components/Text";

import resources from "@ui/resources";

interface IProps {
    // Container properties.
    style?: Style;
    class?: string;

    // Text properties.
    size?: Size;
    children: string;
}

function Button(props: IProps) {
    const [hover, setHover] = useState(false);

    return (
        <button
            class={`${props.class ?? ""} px-16 py-6 border-none border-0 bg-transparent`}
            style={{
                ...props.style,
                backgroundImage: hover && resources.ButtonBackground
            }}
            onMouseOver={() => setHover(true)}
            onMouseOut={() => setHover(false)}
        >
            {/*<Image*/}
            {/*    src={resources}*/}
            {/*/>*/}
            <Text
                class={"text"}
                size={props.size ?? Size.Normal}
            >
                {props.children}
            </Text>
        </button>
    );
}

export default Button;
