import resources from "@ui/resources";
import Text, { Size } from "@components/Text";
import { h } from "preact";

function Key(props: { children: string, class?: string }) {
    return (
        <div
            class={props.class}
            style={{
                backgroundImage: resources.KeyCap,
                width: 126, height: 136
            }}
        >
            <Text
                size={Size.ExtraLarge}
                style={{ left: 5 }}
                class={"text-white text-center self-center"}
            >
                {props.children}
            </Text>
        </div>
    );
}

function Keyboard({ keys }: { keys: string[] }) {
    if (keys.length != 4) return <div />;

    return (
        <div class={"flex-col items-center"}>
            <Key class={"mb-6"}>{keys[0]}</Key>

            <div class={"flex-row"}>
                <Key class={"mr-10"}>{keys[1]}</Key>
                <Key class={"mt-2 mr-10"}>{keys[2]}</Key>
                <Key>{keys[3]}</Key>
            </div>
        </div>
    );
}

export default Keyboard;
