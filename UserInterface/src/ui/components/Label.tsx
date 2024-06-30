import { h } from "preact";

import TextBox from "@components/TextBox";

import * as resources from "@ui/resources";

import { Texture2D } from "UnityEngine";

interface IProps {
    icon: Texture2D;
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
                        width: 129,
                        height: 66,
                        top: 15,
                        right: 20
                    }}
                    image={resources.Flag}
                />

                <TextBox
                    class={"pl-10 pr-14"}
                    icon={props.icon}
                    label={props.children}
                />
            </div>

            <div class={"w-auto"} />
        </div>
    );
}

export default Label;
