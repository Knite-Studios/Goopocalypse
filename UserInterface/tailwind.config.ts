import plugin from "tailwindcss/plugin";

import * as onejs from "./ScriptLib/onejs-tw-config";

export default {
    content: [...onejs.paths, "./src/**/*.{tsx,ts}"],
    theme: onejs.theme,
    plugins: [
        ...onejs.plugins,
        plugin(function ({ addUtilities }) {
            addUtilities({
                ".default-bg-color": { "background-color": "white" },
                ".accented-bg-color": { "background-color": "#fde047" },
                ".hover-bg-color": { "background-color": "rgb(0 0 0 / 0.1)" },
                ".default-text-color": { color: "#4b5563" },
                ".active-text-color": { color: "#cd8c06" },
                ".highlighted-text-color": { color: "#854d0e" }
            });
        })
    ],
    corePlugins: onejs.corePlugins
};
