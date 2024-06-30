import { h } from "preact";
import { Style } from "preact/jsx";
import { JSX } from "preact/jsx-runtime";

interface IProps {
    class?: string;
    style?: Style;

    overlay?: boolean;

    children: JSX.Element | JSX.Element[] | null;
}

function Box(props: IProps) {
    return (
        <div class={"w-full h-full"}>
            { props.overlay && (
                <div class={"w-full h-full bg-dark-blue"} style={{ opacity: 0.7 }} />
            ) }

            <div
                class={`absolute w-full h-full bg-dark-blue rounded-3xl border-4 border-lime left-1/2 top-1/2 ${props.class || ""}`}
                style={{ maxWidth: 1082, maxHeight: 662, translate: "-50%, -50%", ...props.style }}
            >
                {props.children}
            </div>
        </div>
    );
}

export default Box;
