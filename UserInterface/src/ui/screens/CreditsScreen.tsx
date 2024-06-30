import { h } from "preact";
import { useState } from "preact/hooks";

import { ScreenProps } from "@ui/App";
import resources, { gradient } from "@ui/resources";

import Label from "@components/LabelV2";
import Button from "@components/ButtonV2";
import Banner from "@components/Banner";
import Image from "@components/Image";

import { Application, Texture2D } from "UnityEngine";

interface INameProps {
    children: string;
    url: string;
    picture: Texture2D;

    hidden?: boolean;
    class?: string;
}

function Name(props: INameProps) {
    const [hovered, setHovered] = useState(false);

    return (
        <div
            class={props.class}
            style={{
                visibility: props.hidden ? "Hidden" : "Visible"
            }}
        >
            <div
                style={{
                    backgroundImage: resources.CardBackground,
                    width: 230, height: 230
                }}
                class={"self-center justify-center"}
            >
                <Image
                    src={props.picture}
                    class={"self-center"}
                    style={{ width: 184, height: 184 }}
                />
            </div>

            <Banner background={resources.NameFrame}
                    minWidth={300}
                    class={"bottom-3 mb-2"}
            >
                {props.children}
            </Banner>

            <Image
                class={"self-center"}
                src={hovered ? resources.ActiveLinkedIn : resources.LinkedIn}
                style={{ width: 39, height: 36 }}
                onMouseOver={() => setHovered(true)}
                onMouseOut={() => setHovered(false)}
                onClick={() => Application.OpenURL(props.url)}
            />
        </div>
    );
}

function CreditsScreen({ navigate }: ScreenProps) {
    return (
        <div class={"w-full h-full bg-dark-blue"}>
            <gradientrect
                vertical
                class={"absolute bottom-0 w-full h-[50%]"}
                colors={gradient}
            />

            <div class={"w-full h-full"}>
                <Label class={"mt-14 mb-[5%]"}>Credits</Label>

                <div class={"flex-col mb-16"}>
                    <div class={"flex-row justify-between px-24 mb-12"}>
                        <Name picture={resources.Aeonamuse}
                              url={"https://www.linkedin.com/in/walter-wong-8641b325a/"}
                        >
                            Aeonamuse
                        </Name>

                        <Name picture={resources.Taiga74164}
                              url={"https://www.linkedin.com/in/joaquin74164/"}
                        >
                            Taiga74164
                        </Name>

                        <Name picture={resources.RetroSensei}
                              url={"https://www.linkedin.com/in/leonardobaldicera/"}
                        >
                            RetroSensei
                        </Name>

                        <Name picture={resources.Perz}
                              url={"https://www.linkedin.com/in/perzeus/"}
                        >
                            Perz
                        </Name>
                    </div>

                    <div class={"flex-row justify-between px-24"}>
                        <Name picture={resources.KingRainbow44}
                              url={"https://www.linkedin.com/in/kobe-do-62700a229/"}
                        >
                            KingRainbow44
                        </Name>

                        <Name picture={resources.Artmanoil}
                              url={"https://linkedin.com/"}
                        >
                            Artmanoil
                        </Name>

                        <Name
                            hidden
                            picture={resources.Placeholder}
                            url={"https://youtube.com/@KingRainbow44"}
                        >
                            Why are you here?
                        </Name>

                        <Name
                            hidden
                            picture={resources.Placeholder}
                            url={"https://seikimo.moe"}
                        >
                            Hello World!
                        </Name>
                    </div>
                </div>

                <Button
                    class={"absolute w-72 bottom-14 right-10"}
                    onClick={() => navigate("/")}
                >
                    Back
                </Button>
            </div>
        </div>
    );
}

export default CreditsScreen;
