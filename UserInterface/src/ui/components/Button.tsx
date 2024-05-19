import { h } from "preact";

import Text, { Size } from "@components/Text";

interface IProps {
    children: string;
    class?: string;

    textSize?: Size;

    onClick?: () => void;
}

function Button(props: IProps) {
    return (
        <button
            class={props.class}
            onClick={props.onClick}
        >
            <Text size={props.textSize}>{props.children}</Text>
        </button>
    );
}

export default Button;
