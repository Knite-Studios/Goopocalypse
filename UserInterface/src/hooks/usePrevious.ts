import { useEffect, useRef } from "preact/hooks";

function usePrevious<T>(value: T): T {
    const ref = useRef<T>();

    useEffect(() => {
        ref.current = value;
    });

    return ref.current;
}

export default usePrevious;
