import { h, cloneElement, JSX } from "preact";

import { Children } from "preact/compat";

interface IRouteProps {
    path: string;
    element: JSX.Element;
}
type _IRouteProps = IRouteProps & {
    isActive: boolean;
    navigate: (route: string) => void;
};

export function Route(props: IRouteProps) {
    const { element, navigate, isActive } = props as _IRouteProps;

    return isActive ? cloneElement(element, { navigate }) : <div />;
}

interface IProps {
    route: string;
    setRoute: (route: string) => void;

    children: JSX.Element[];
}

function Router(props: IProps) {
    return (
        <div class={"w-full h-full"}>
            { Children.map(props.children, (child: any) => {
                const addons: any = { navigate: props.setRoute };
                addons.isActive = (child as JSX.Element).props.path == props.route;

                return cloneElement(child, addons);
            }) }
        </div>
    );
}

export default Router;
