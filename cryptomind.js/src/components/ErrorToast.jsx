import { useState, useEffect, useCallback } from "react";

const TOAST_DURATION = 5000;

const AlertIcon = () => (
    <svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
        style={{ width: "1.1rem", height: "1.1rem" }}
    >
        <circle cx="12" cy="12" r="10" />
        <line x1="12" y1="8" x2="12" y2="12" />
        <line x1="12" y1="16" x2="12.01" y2="16" />
    </svg>
);

const CloseIcon = () => (
    <svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="2.5"
        strokeLinecap="round"
        strokeLinejoin="round"
        style={{ width: "0.9rem", height: "0.9rem" }}
    >
        <line x1="18" y1="6" x2="6" y2="18" />
        <line x1="6" y1="6" x2="18" y2="18" />
    </svg>
);

/**
 * ErrorToast component
 *
 * Usage:
 *   const [error, setError] = useState(null);
 *   <ErrorToast message={error} onDismiss={() => setError(null)} />
 *
 * To trigger:  setError("Something went wrong.")
 * To dismiss:  setError(null)   ← happens automatically after 5s too
 */
export default function ErrorToast({ message, onDismiss, duration = TOAST_DURATION }) {
    const [visible, setVisible] = useState(false);
    const [dismissing, setDismissing] = useState(false);

    // Animate in whenever a new message arrives
    useEffect(() => {
        if (!message) return;
        setDismissing(false);
        setVisible(true);

        const timer = setTimeout(() => handleDismiss(), duration);
        return () => clearTimeout(timer);
    }, [message, duration]);

    const handleDismiss = useCallback(() => {
        setDismissing(true);
        setTimeout(() => {
            setVisible(false);
            setDismissing(false);
            onDismiss?.();
        }, 300); // matches slide-out animation
    }, [onDismiss]);

    if (!visible || !message) return null;

    return (
        <>
            <style>{styles(duration)}</style>

            <div
                className={`error-toast ${dismissing ? "error-toast--dismiss" : ""}`}
                role="alert"
                aria-live="assertive"
            >
                <div className="error-toast__icon">
                    <AlertIcon />
                </div>

                <div className="error-toast__body">
                    <span className="error-toast__title">Error</span>
                    <span className="error-toast__message">{message}</span>
                </div>

                <button
                    className="error-toast__close"
                    onClick={handleDismiss}
                    aria-label="Dismiss error"
                >
                    <CloseIcon />
                </button>

                <div className="error-toast__progress" />
            </div>
        </>
    );
}

// Styles are injected as a function so the duration CSS var stays in sync
const styles = (duration) => `
    .error-toast {
        position: fixed;
        bottom: 2rem;
        right: 2rem;
        z-index: 9999;

        display: flex;
        align-items: center;
        gap: 0.875rem;

        min-width: 320px;
        max-width: 460px;

        padding: 1rem 1.125rem 1.35rem;

        background-color: #1a1a2e;
        border: 1px solid #ff4d4f;
        border-left: 4px solid #ff4d4f;
        border-radius: 10px;

        box-shadow:
            0 8px 32px rgba(255, 77, 79, 0.18),
            0 2px 8px rgba(0, 0, 0, 0.45);

        overflow: hidden;
        animation: toast-slide-in 0.35s cubic-bezier(0.22, 1, 0.36, 1) forwards;
    }

    .error-toast--dismiss {
        animation: toast-slide-out 0.3s ease forwards;
    }

    .error-toast__icon {
        flex-shrink: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 2rem;
        height: 2rem;
        border-radius: 50%;
        background-color: rgba(255, 77, 79, 0.15);
        color: #ff4d4f;
    }

    .error-toast__body {
        flex: 1;
        display: flex;
        flex-direction: column;
        gap: 0.2rem;
        min-width: 0;
    }

    .error-toast__title {
        font-size: 0.78rem;
        font-weight: 700;
        letter-spacing: 0.08em;
        text-transform: uppercase;
        color: #ff4d4f;
    }

    .error-toast__message {
        font-size: 0.92rem;
        color: #e0e0e0;
        line-height: 1.4;
        word-break: break-word;
    }

    .error-toast__close {
        flex-shrink: 0;
        align-self: flex-start;
        display: flex;
        align-items: center;
        justify-content: center;
        width: 1.6rem;
        height: 1.6rem;
        padding: 0;
        background: transparent;
        border: none;
        border-radius: 4px;
        color: #888;
        cursor: pointer;
        transition: color 0.2s, background-color 0.2s;
    }

    .error-toast__close:hover {
        color: #ff4d4f;
        background-color: rgba(255, 77, 79, 0.1);
    }

    .error-toast__progress {
        position: absolute;
        bottom: 0;
        left: 0;
        height: 3px;
        width: 100%;
        background-color: #ff4d4f;
        border-radius: 0 0 10px 10px;
        transform-origin: left;
        animation: toast-progress ${duration}ms linear forwards;
    }

    @keyframes toast-slide-in {
        from { opacity: 0; transform: translateX(110%) scale(0.95); }
        to   { opacity: 1; transform: translateX(0)    scale(1);    }
    }

    @keyframes toast-slide-out {
        from { opacity: 1; transform: translateX(0)    scale(1);    }
        to   { opacity: 0; transform: translateX(110%) scale(0.95); }
    }

    @keyframes toast-progress {
        from { transform: scaleX(1); }
        to   { transform: scaleX(0); }
    }

    @media (max-width: 480px) {
        .error-toast {
            bottom: 1rem;
            right: 1rem;
            left: 1rem;
            min-width: unset;
            max-width: unset;
        }
    }
`;