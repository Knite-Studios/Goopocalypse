import { h } from "preact";
import { useEffect, useRef, useState } from "preact/hooks";
import { Style } from "preact/jsx";

import Image from "@components/Image";

interface IProps {
    start: number; // This is the number of the first frame.
    end: number; // This is the number of the last frame.

    frames: string; // This is a path from 'resources/' to the folder containing all GIF frames.
    prefix: string; // This is the prefix of the GIF frames. (ex. "Fwend")
    fps: number; // The amount of frames per second.

    style?: Style;
    class?: string;
}

function GifRenderer(props: IProps) {
    const [frame, setFrame] = useState(props.start);
    const frameRef = useRef<number>(frame);

    const timeout = 1000 / props.fps;

    useEffect(() => {
        function updateFrame() {
            const thisFrame = frameRef.current;
            if (thisFrame == props.end) {
                setFrame(props.start);
            } else {
                setFrame(thisFrame + 1);
            }
            setTimeout(updateFrame, timeout);
        }
        updateFrame();
    }, []);

    useEffect(() => {
        frameRef.current = frame;
    }, [frame]);

    const image = resource.loadImage(
        `${__dirname}/resources/${props.frames}/${props.prefix}${frame}.png`
    );

    return (
        <Image style={props.style} class={props.class}>
            {image}
        </Image>
    );
}

export default GifRenderer;
