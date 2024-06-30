import { h } from "preact";

import { Texture2D } from "UnityEngine";
import Text from "@components/Text";
import resources from "@ui/resources";
import Image from "@components/Image";
import Banner from "@components/Banner";

interface IProps {
    title?: string; // This is displayed above the card.

    content: string; // This is displayed in the lower box.
    picture: Texture2D; // This is displayed in the upper box.

    icon?: Texture2D; // This is displayed below the card.

    class?: string;
}

function Card(props: IProps) {
    const { title, icon } = props;

    return (
        <div class={`flex-col items-center ${props.class ?? ""}`}>
            { title && <Text class={"text-white mb-8"}>{title}</Text> }

            <div
                class={"p-8 mb-8"}
                style={{ backgroundImage: resources.CardBackground }}
            >
                <Image style={{ width: 250, height: 250 }}>
                    {props.picture}
                </Image>
            </div>

            <Banner class={"mb-8"}>{props.content}</Banner>
        </div>
    );
}

export default Card;
