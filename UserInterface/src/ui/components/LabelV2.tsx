import { h } from "preact";

import TextBox from "@components/TextBoxV2";

import * as resources from "@ui/resources";

interface IProps {
    children: string;

    class?: string;
}

function Label(props: IProps) {
    return (
        <div class={`flex-row ${props.class || ""}`}>
            <div class={"flex-row-reverse"}>
                <image
                    class={"self-end"}
                    style={{
                        top: 20,
                        right: 120
                    }}
                    image={resources.Flag}
                />

                <TextBox>{props.children}</TextBox>
            </div>

            <div class={"w-auto"} />
        </div>
    );
}

export default Label;
