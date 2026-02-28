import '../styles/notification-item.css';

import { NotificationType, parseCreatedSince } from './UseNotifications';

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

const FALLBACK_CONFIG = {
    variant: 'info',
    icon: <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>,
};

const NotificationItem = ({ notification, onClick }) => {
    const config = TYPE_CONFIG[notification.type] ?? FALLBACK_CONFIG;

    const classes = [
        'nm-item',
        `nm-item--${config.variant}`,
        !notification.isRead ? 'nm-item--unread' : '',
    ].filter(Boolean).join(' ');

    const handleClick = () => onClick?.(notification);

    return (
        <li
            className={classes}
            onClick={handleClick}
            role="button"
            tabIndex={0}
            onKeyDown={e => e.key === 'Enter' && handleClick()}
        >
            <span className="nm-item__bar" aria-hidden="true" />

            <span className="nm-item__icon">
                {config.icon}
            </span>

            <div className="nm-item__body">
                <p className="nm-item__message">{notification.message}</p>
                <div className="nm-item__meta">
                    <span className="nm-item__time">
                        {parseCreatedSince(notification.createdSince)}
                    </span>
                    {!notification.isRead && (
                        <span className="nm-item__unread-dot" aria-label="Unread" />
                    )}
                </div>
            </div>
        </li>
    );
};

export default NotificationItem;