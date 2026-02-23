import { createContext, useContext, useState, useCallback } from "react";
import ErrorToast from "./components/ErrorToast";

const ErrorContext = createContext(null);

export function ErrorProvider({ children }) {
    const [error, setErrorState] = useState(null);

    const setError = useCallback((message) => {
        setErrorState(message);
    }, []);

    const clearError = useCallback(() => {
        setErrorState(null);
    }, []);

    return (
        <ErrorContext.Provider value={{ setError, clearError }}>
            {children}
            <ErrorToast message={error} onDismiss={clearError} />
        </ErrorContext.Provider>
    );
}

export function useError() {
    const context = useContext(ErrorContext);
    if (!context) {
        throw new Error("useError must be used inside an ErrorProvider");
    }
    return context;
}