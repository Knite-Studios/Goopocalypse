import { parseColor } from "onejs/utils/color-parser";

export const ThaleahFat = resource.loadFont(
    `${__dirname}/resources/ThaleahFat.ttf`
);

export const Logo = resource.loadImage(`${__dirname}/resources/logo.png`);
export const KeyCap = resource.loadImage(`${__dirname}/resources/keycap.png`);
export const Flag = resource.loadImage(`${__dirname}/resources/flag.png`);
export const MiniFlag = resource.loadImage(
    `${__dirname}/resources/mini_flag.png`
);
export const ButtonBackground = resource.loadImage(
    `${__dirname}/resources/button_bg.png`
);
export const LabelBackground = resource.loadImage(
    `${__dirname}/resources/label_bg.png`
);
export const CardBackground = resource.loadImage(
    `${__dirname}/resources/card_bg.png`
);
export const ProfileFrame = resource.loadImage(
    `${__dirname}/resources/profile_frame.png`
);
export const NameFrame = resource.loadImage(
    `${__dirname}/resources/name_frame.png`
);

export const Tutorial = resource.loadImage(
    `${__dirname}/resources/tutorial.png`
);

export const LinkedIn = resource.loadImage(
    `${__dirname}/resources/linkedin.png`
);
export const ActiveLinkedIn = resource.loadImage(
    `${__dirname}/resources/active_linkedin.png`
);
export const Placeholder = resource.loadImage(
    `${__dirname}/resources/people/placeholder.png`
);

export const gradient = [
    parseColor("rgba(81, 213, 152, 0.6)"),
    parseColor("rgba(14, 34, 41, 0)")
];

export default {
    ThaleahFat,
    Logo,
    KeyCap,
    Flag,
    MiniFlag,
    ProfileFrame,
    NameFrame,
    ButtonBackground,
    LabelBackground,
    CardBackground,
    gradient,
    Tutorial,
    Placeholder,
    LinkedIn,
    ActiveLinkedIn
};
