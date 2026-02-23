import '../styles/toast.css';
import { NotificationType } from './UseNotifications';

/*
 * How auto-dismiss works:
 *
 * The parent (ToastContainer) calls dismissToast(id) after DISMISS_MS.
 * Before removing the toast from state it toggles .toast--leaving which
 * plays the exit animation. The CSS animation duration matches DISMISS_MS
 * so the progress bar empties exactly as the card leaves.
 *
 * This value is read here only for the CSS variable — the actual timer
 * lives in ToastContainer.
 */
export const DISMISS_MS = 4500;

// Same type → variant + icon mapping used in NotificationItem
const TYPE_CONFIG = {
    0: { variant: 'approved', label: 'Cipher approved',  icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="20 6 9 17 4 12"/></svg> },
    1: { variant: 'rejected', label: 'Cipher rejected',  icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg> },
    2: { variant: 'rejected', label: 'Cipher removed',   icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14H6L5 6"/><path d="M10 11v6"/><path d="M14 11v6"/></svg> },
    3: { variant: 'info',     label: 'Cipher restored',  icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg> },
    4: { variant: 'info',     label: 'Cipher updated',   icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg> },
    5: { variant: 'approved', label: 'Answer approved',  icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><polyline points="20 6 9 17 4 12"/></svg> },
    6: { variant: 'rejected', label: 'Answer rejected',  icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg> },
    7: { variant: 'info',     label: 'Cipher removed',   icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg> },
    8: { variant: 'info',     label: 'Cipher restored',  icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="23 4 23 10 17 10"/><path d="M20.49 15a9 9 0 1 1-2.12-9.36L23 10"/></svg> },
    9: { variant: 'badge',    label: 'Badge earned',     icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="8" r="6"/><path d="M15.477 12.89L17 22l-5-3-5 3 1.523-9.11"/></svg> },
};

const FALLBACK = {
    variant: 'info',
    label: 'Notification',
    icon: <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>,
};

/*
 * Toast
 *
 * Props:
 *   toast       — { id, notification, isLeaving }
 *   onDismiss   — called with toast.id when X is clicked
 *   onClick     — called with the notification when the card body is clicked
 */
const Toast = ({ toast, onDismiss, onClick }) => {
    const config = TYPE_CONFIG[toast.notification.type] ?? FALLBACK;

    const handleBodyClick = () => {
        onDismiss(toast.id);
        onClick(toast.notification);
    };

    const handleClose = (e) => {
        e.stopPropagation(); // don't trigger body click
        onDismiss(toast.id);
    };

    return (
        <div
            className={[
                'toast',
                `toast--${config.variant}`,
                toast.isLeaving ? 'toast--leaving' : '',
            ].filter(Boolean).join(' ')}
            onClick={handleBodyClick}
            role="alert"
            aria-live="polite"
        >
            <span className="toast__stripe" aria-hidden="true" />

            <span className="toast__icon">{config.icon}</span>

            <div className="toast__body">
                <p className="toast__label">{config.label}</p>
                <p className="toast__message">{toast.notification.message}</p>
            </div>

            <button
                className="toast__close"
                onClick={handleClose}
                aria-label="Dismiss notification"
            >
                <svg width="12" height="12" viewBox="0 0 24 24" fill="none"
                    stroke="currentColor" strokeWidth="2.5"
                    strokeLinecap="round" strokeLinejoin="round">
                    <line x1="18" y1="6" x2="6" y2="18"/>
                    <line x1="6" y1="6" x2="18" y2="18"/>
                </svg>
            </button>

            {/* Draining progress bar */}
            <span
                className="toast__progress"
                style={{ '--toast-duration': `${DISMISS_MS}ms` }}
                aria-hidden="true"
            />
        </div>
    );
};

export default Toast;