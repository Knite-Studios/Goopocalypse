import { h } from "preact";

export enum Size {
    Footnote = 16,
    Small = 24,
    Normal = 36,
    Large = 48,
    Title = 64
}

interface IProps {
    class?: string;

    size?: Size;
    bold?: boolean;
    italic?: boolean;

    children: string;
}

function Text(props: IProps) {
    return (
        <span
            class={props.class}
            style={{
                fontSize: props.size || Size.Normal,
                unityFontStyleAndWeight: props.bold && props.italic ? "BoldAndItalic" :
                    props.bold ? "Bold" : props.italic ? "Italic" : "Normal"
            }}
        >
            {props.children}
        </span>
    );
}

export default Text;
