import { h } from "preact";

import type { ScriptManager } from "game";

function MenuScreen({ game }: { game: ScriptManager }) {
    return (
        <div>
            <div>

            </div>

            <div>
                <div>
                    <button>
                        Credits
                    </button>

                    <button>
                        Select
                    </button>
                </div>
            </div>
        </div>
    );
}

export default MenuScreen;
