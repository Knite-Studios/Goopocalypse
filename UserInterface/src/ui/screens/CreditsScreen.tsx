import { h } from "preact";
import { useState } from "preact/hooks";

import { ScreenProps } from "@ui/App";
import resources, { gradient, Icons } from "@ui/resources";

import Label from "@components/LabelV2";
import Button from "@components/ButtonV2";
import Banner from "@components/Banner";
import Image from "@components/Image";

import { Application, Texture2D } from "UnityEngine";

interface IIconProps {
    url: string;
    icon: Texture2D[];
}

function Icon({ url, icon }: IIconProps) {
    const [hovered, setHovered] = useState(false);

    return (
        <Image
            class={"self-center"}
            src={hovered ? icon[1] : icon[0]}
            style={{ width: 39, height: 36 }}
            onMouseOver={() => setHovered(true)}
            onMouseOut={() => setHovered(false)}
            onClick={() => Application.OpenURL(url)}
        />
    );
}

interface INameProps {
    children: string;
    urls: string[];
    icons: Texture2D[]; // This contains both the active and inactive states.
    picture: Texture2D;

    hidden?: boolean;
    class?: string;
}

function Name(props: INameProps) {
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

            <div class={"self-center flex-row"}>
                <Icon url={props.urls[0]} icon={[
                    props.icons[0], props.icons[1]
                ]} />

                { props.urls.length == 2 && <div class={"ml-5"}>
                    <Icon url={props.urls[1]} icon={[
                        props.icons[2], props.icons[3]
                    ]} />
                </div> }
            </div>
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
                              urls={["https://www.linkedin.com/in/walter-wong-8641b325a/"]}
                              icons={[Icons.LinkedIn, Icons.ActiveLinkedIn]}
                        >
                            Aeonamuse
                        </Name>

                        <Name picture={resources.Taiga74164}
                              urls={["https://www.linkedin.com/in/joaquin74164/", "https://github.com/Taiga74164"]}
                              icons={[Icons.LinkedIn, Icons.ActiveLinkedIn, Icons.GitHub, Icons.ActiveGitHub]}
                        >
                            Taiga74164
                        </Name>

                        <Name picture={resources.RetroSensei}
                              urls={["https://www.linkedin.com/in/leonardobaldicera/"]}
                              icons={[Icons.LinkedIn, Icons.ActiveLinkedIn]}
                        >
                            RetroSensei
                        </Name>

                        <Name picture={resources.Perz}
                              urls={["https://www.linkedin.com/in/perzeus/", "https://github.com/PerzVT"]}
                              icons={[Icons.LinkedIn, Icons.ActiveLinkedIn, Icons.GitHub, Icons.ActiveGitHub]}
                        >
                            Perz
                        </Name>
                    </div>

                    <div class={"flex-row justify-between px-24"}>
                        <Name
                            hidden
                            picture={resources.Placeholder}
                            urls={["https://youtube.com/@KingRainbow44"]}
                            icons={[Icons.GitHub, Icons.ActiveGitHub]}
                        >
                            Why are you here?
                        </Name>

                        <Name picture={resources.KingRainbow44}
                              urls={["https://github.com/KingRainbow44"]}
                              icons={[Icons.GitHub, Icons.ActiveGitHub]}
                        >
                            KingRainbow44
                        </Name>

                        <Name picture={resources.Artmanoil}
                              urls={["https://x.com/"]}
                              icons={[Icons.Twitter, Icons.ActiveTwitter]}
                        >
                            Artmanoil
                        </Name>

                        <Name
                            hidden
                            picture={resources.Placeholder}
                            urls={["https://seikimo.moe"]}
                            icons={[Icons.GitHub, Icons.ActiveGitHub]}
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
