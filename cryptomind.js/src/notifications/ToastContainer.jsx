import '../styles/toast-container.css';
import Toast, { DISMISS_MS } from './Toast';
import { useEffect, useRef } from 'react';

const ToastContainer = ({ toasts, onDismiss, onNotificationClick }) => {
    // Track running timers so we can cancel if the user dismisses early
    const timersRef = useRef({});

    useEffect(() => {
        toasts.forEach(toast => {
            // Only start a timer for toasts that don't have one yet
            if (!timersRef.current[toast.id] && !toast.isLeaving) {
                timersRef.current[toast.id] = setTimeout(() => {
                    onDismiss(toast.id);
                    delete timersRef.current[toast.id];
                }, DISMISS_MS);
            }
        });

        // Clean up timers for toasts that were manually dismissed
        const activeIds = new Set(toasts.map(t => t.id));
        Object.keys(timersRef.current).forEach(id => {
            if (!activeIds.has(id)) {
                clearTimeout(timersRef.current[id]);
                delete timersRef.current[id];
            }
        });
    }, [toasts, onDismiss]);

    // Clear all timers on unmount
    useEffect(() => {
        return () => {
            Object.values(timersRef.current).forEach(clearTimeout);
        };
    }, []);

    if (toasts.length === 0) return null;

    return (
        <div className="toast-container" aria-label="Notifications">
            {toasts.map(toast => (
                <Toast
                    key={toast.id}
                    toast={toast}
                    onDismiss={onDismiss}
                    onClick={onNotificationClick}
                />
            ))}
        </div>
    );
};

export default ToastContainer;