import { h } from "preact";

import Text from "@components/Text";

import resources from "@ui/resources";

interface IProps {
    children: string;
}

function TextBox(props: IProps) {
    return (
        <div
            class={"flex-col items-center justify-center"}
            style={{
                minWidth: 469,
                minHeight: 110,
                backgroundImage: resources.LabelBackground
            }}
        >
            <Text class={"text-white"}>{props.children}</Text>
        </div>
    );
}

export default TextBox;
