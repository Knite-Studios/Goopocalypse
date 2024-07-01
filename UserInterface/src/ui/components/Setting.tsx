import { h } from "preact";
import { JSX } from "preact/jsx-runtime";

import Text from "@components/Text";
import Image from "@components/Image";

import resources from "@ui/resources";

import { ScriptManager } from "game";

const { AudioManager } = require("game") as ScriptManager;

/**
 * Creates an array of numbers from 0 to size - 1.
 *
 * @param size The size of the array.
 */
function array(size: number): number[] {
    return Array.from({ length: size }, (_, i) => i);
}

interface IProps {
    name: string;
    class?: string;

    /**
     * string for options; should be the displayed (and current) value
     * number for slider; should be the index of the value
     */
    current: string | number;

    type: "options" | "slider";

    /**
     * string[] for options, number[] for slider.
     *
     * string[] has a minimum size of 1
     * number[] should contain exactly 10 elements
     */
    values: string[] | number[];

    onChange: (value: string | number) => void;
}

function Setting(props: IProps) {
    let selector: JSX.Element;
    switch (props.type) {
        case "options":
            selector = (
                <div class={"flex-row"}>
                    <Image
                        class={"mr-5"}
                        onClick={() => {
                            const currentIndex = props.values.indexOf(props.current);
                            const newIndex = currentIndex == 0 ?
                                props.values.length - 1 :
                                currentIndex - 1;

                            props.onChange(props.values[newIndex]);

                            AudioManager.PlayUIClickSound();
                        }}
                        onMouseOver={() => AudioManager.PlayUIHoverSound()}
                        src={resources.Arrow}
                    />

                    <Text class={"text-white mr-5"}>{props.current}</Text>

                    <Image
                        onClick={() => {
                            const currentIndex = props.values.indexOf(props.current);
                            const newIndex = currentIndex == props.values.length - 1 ?
                                -1 :
                                currentIndex + 1;

                            props.onChange(props.values[newIndex]);

                            AudioManager.PlayUIClickSound();
                        }}
                        onMouseOver={() => AudioManager.PlayUIHoverSound()}
                        src={resources.Arrow}
                        style={{ rotate: 180 }}
                    />
                </div>
            );
            break;
        case "slider":
            const currentIndex = props.current as number;

            selector = (
                <div class={"flex-row"}>
                    <Image
                        class={"mr-5"}
                        onClick={() => {
                            const newIndex = currentIndex == -1 ?
                                9 :
                                currentIndex - 1;
                            props.onChange(props.values[newIndex]);

                            AudioManager.PlayUIClickSound();
                        }}
                        onMouseOver={() => AudioManager.PlayUIHoverSound()}
                        src={resources.Arrow}
                    />

                    <div class={"w-full flex-row"}>
                        { array(10).map((_, key) => (
                            <Image
                                class={key == 10 ? "" : "mr-3"}
                                onClick={() => {
                                    props.onChange(props.values[key]);

                                    AudioManager.PlayUIClickSound();
                                }}
                                onMouseOver={() => AudioManager.PlayUIHoverSound()}
                                src={key <= currentIndex ? resources.Set : resources.Unset}
                            />
                        )) }
                    </div>

                    <Image
                        onClick={() => {
                            const newIndex = currentIndex == 9 ?
                                -1 :
                                currentIndex + 1;
                            props.onChange(props.values[newIndex]);

                            AudioManager.PlayUIClickSound();
                        }}
                        onMouseOver={() => AudioManager.PlayUIHoverSound()}
                        src={resources.Arrow}
                        style={{ rotate: 180 }}
                    />
                </div>
            );
            break;
    }

    return (
        <div class={`w-full flex-row justify-between ${props.class || ""}`}>
            <Text class={"text-white"}>{props.name}</Text>

            {selector}
        </div>
    );
}

export default Setting;
