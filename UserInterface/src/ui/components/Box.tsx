import { h, JSX } from "preact";

interface IProps {
    children: JSX.Element | JSX.Element[];
}

function Box(props: IProps) {
    return (
        <div class={"items-center flex-row justify-between"}>
            <div />
            {props.children}
            <div />
        </div>
    );
}

export default Box;
