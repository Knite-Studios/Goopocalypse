import { h } from "preact";

import { ScreenProps } from "@ui/App";
import { gradient } from "@ui/resources";
import Banner from "@components/Banner";
import { Size } from "@components/Text";

function WaveScreen(_props: ScreenProps) {
    return (
        <div class={"w-full h-full"}>
            <gradientrect
                vertical
                class={"absolute h-full left-40"}
                style={{ width: 470 }}
                // This gradient is actually flipped.
                colors={[gradient[1], gradient[0]]}
            >
                <Banner
                    class={"mt-32"}
                    textClass={"py-5"}
                    flagClass={"scale-150"}
                    size={Size.Normal}
                >
                    Survive!
                </Banner>
            </gradientrect>
        </div>
    );
}

export default WaveScreen;
