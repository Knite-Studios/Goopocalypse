import { h, JSX } from "preact";

interface IProps {
    color: string;
    radius: number;

    children?: JSX.Element | JSX.Element[];
}

function Shadow(props: IProps) {
    const { color, radius } = props;

    return (
        <div class={"flex-col"}>
            <div
                class={"absolute w-full h-full"}
                style={{
                    top: radius,
                    left: radius,
                    backgroundColor: color
                }}
            />

            {props.children}
        </div>
    );
}

export default Shadow;
