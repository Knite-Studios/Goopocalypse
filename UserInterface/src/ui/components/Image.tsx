import { h } from "preact";
import { Style } from "preact/jsx";

import { Texture2D } from "UnityEngine";

interface IProps {
    style?: Style;
    class?: string;

    src?: Texture2D;
    children?: Texture2D;

    onMouseOver?: () => void;
    onMouseOut?: () => void;
    onClick?: () => void;
}

function Image(props: IProps) {
    return (
        <image
            style={props.style}
            class={props.class}
            image={props.src ?? props.children}

            onClick={props.onClick}
            onMouseOver={props.onMouseOver}
            onMouseOut={props.onMouseOut}
        />
    );
}

export default Image;
