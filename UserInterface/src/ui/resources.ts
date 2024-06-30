import { parseColor } from "onejs/utils/color-parser";

export const ThaleahFat = resource.loadFont(
    `${__dirname}/resources/ThaleahFat.ttf`
);

export const Logo = resource.loadImage(`${__dirname}/resources/logo.png`);
export const KeyCap = resource.loadImage(`${__dirname}/resources/keycap.png`);
export const Flag = resource.loadImage(`${__dirname}/resources/flag.png`);
export const FullFlag = resource.loadImage(`${__dirname}/resources/full_flag.png`);
export const BackwardsFlag = resource.loadImage(`${__dirname}/resources/back_flag.png`);
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
export const ScoreCard = resource.loadImage(
    `${__dirname}/resources/score_card.png`
);
export const BackwardsScoreCard = resource.loadImage(
    `${__dirname}/resources/backwards_score.png`
);

export const Tutorial = resource.loadImage(
    `${__dirname}/resources/tutorial.png`
);
export const Arrow = resource.loadImage(
    `${__dirname}/resources/arrow.png`
);
export const Set = resource.loadImage(
    `${__dirname}/resources/set.png`
);
export const Unset = resource.loadImage(
    `${__dirname}/resources/unset.png`
);

export const Placeholder = resource.loadImage(
    `${__dirname}/resources/people/placeholder.png`
);
export const Aeonamuse = resource.loadImage(
    `${__dirname}/resources/people/aeonamuse.png`
);
export const Artmanoil = resource.loadImage(
    `${__dirname}/resources/people/artmanoil.png`
);
export const KingRainbow44 = resource.loadImage(
    `${__dirname}/resources/people/kingrainbow44.png`
);
export const Perz = resource.loadImage(
    `${__dirname}/resources/people/perz.png`
);
export const RetroSensei = resource.loadImage(
    `${__dirname}/resources/people/retrosensei.png`
);
export const Taiga74164 = resource.loadImage(
    `${__dirname}/resources/people/taiga74164.png`
);

export const Icons = {
    LinkedIn: resource.loadImage(`${__dirname}/resources/icons/linkedin.png`),
    ActiveLinkedIn: resource.loadImage(
        `${__dirname}/resources/icons/a_linkedin.png`
    ),
    GitHub: resource.loadImage(`${__dirname}/resources/icons/github.png`),
    ActiveGitHub: resource.loadImage(
        `${__dirname}/resources/icons/a_github.png`
    ),
    Twitter: resource.loadImage(`${__dirname}/resources/icons/x.png`),
    ActiveTwitter: resource.loadImage(
        `${__dirname}/resources/icons/a_x.png`
    )
};

export const gradient = [
    parseColor("rgba(81, 213, 152, 0.6)"),
    parseColor("rgba(14, 34, 41, 0)")
];

export default {
    ThaleahFat,
    Logo,
    KeyCap,
    Flag,
    BackwardsFlag,
    MiniFlag,
    ProfileFrame,
    NameFrame,
    ButtonBackground,
    LabelBackground,
    CardBackground,
    gradient,
    Tutorial,
    Placeholder,
    Icons,
    Aeonamuse,
    Artmanoil,
    KingRainbow44,
    Perz,
    RetroSensei,
    Taiga74164,
    ScoreCard,
    Arrow,
    Set,
    Unset,
    FullFlag,
    BackwardsScoreCard
};
