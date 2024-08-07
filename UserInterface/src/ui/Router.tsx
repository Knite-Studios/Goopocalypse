import { h, cloneElement, JSX } from "preact";
import { Children } from "preact/compat";
import { useState } from "preact/hooks";

import { ScreenProps } from "@ui/App";

import { MenuState } from "@type/enums";

import { ScriptManager } from "game";

interface IRouteProps {
    path: string;
    element: (props: ScreenProps) => JSX.Element;
}
type _IRouteProps = IRouteProps & {
    isActive: boolean;

    game: ScriptManager;
    menuState: MenuState;

    lastPage: string;
    setMenuState: (state: MenuState) => void;
    navigate: (route: string) => void;
};

export function Route(_props: IRouteProps) {
    const props = _props as _IRouteProps;
    const { element: Instance } = props;

    return props.isActive ? (
        <Instance
            lastPage={props.lastPage}
            navigate={props.navigate}
            game={props.game}
            menuState={props.menuState}
            setMenuState={props.setMenuState}
        />
    ) : (
        <div />
    );
}

interface IProps {
    route: string;
    previous: string;
    setRoute: (route: string) => void;

    game: ScriptManager;
    children: JSX.Element[];
}

function Router(props: IProps) {
    const [menuState, setMenuState] = useState(MenuState.Local);

    return (
        <div class={"w-full h-full"}>
            {Children.map(props.children, (child: any) => {
                const addons: any = {
                    navigate: props.setRoute,
                    game: props.game,
                    menuState,
                    setMenuState,
                    lastPage: props.previous
                };
                addons.isActive =
                    (child as JSX.Element).props.path == props.route;

                return cloneElement(child, addons);
            })}
        </div>
    );
}

export default Router;
