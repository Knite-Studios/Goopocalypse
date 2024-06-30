import { h } from "preact";
import { Style } from "preact/jsx";

import * as resources from "@ui/resources";

export enum Size {
    Small = 40,
    Normal = 64,
    Large = 96,
    ExtraLarge = 128
}

interface IProps {
    class?: string;
    style?: Style;

    size?: Size;
    bold?: boolean;
    italic?: boolean;

    children: string | string[] | undefined;
}

function Text(props: IProps) {
    return (
        <span
            class={props.class}
            style={{
                fontSize: props.size || Size.Normal,
                unityFontStyleAndWeight:
                    props.bold && props.italic
                        ? "BoldAndItalic"
                        : props.bold
                        ? "Bold"
                        : props.italic
                        ? "Italic"
                        : "Normal",
                unityFontDefinition: resources.ThaleahFat,
                ...props.style
            }}
        >
            {props.children}
        </span>
    );
}

export default Text;
