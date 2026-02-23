import { useState, useEffect, useRef } from 'react';
import '../styles/notifications-bell.css';
import { useNotificationContext } from '../App.jsx';
import NotificationDropdown from './NotificationDropdown';

const PREVIEW_COUNT = 5;

const NotificationBell = () => {
    const [isOpen, setIsOpen]     = useState(false);
    const [didShake, setDidShake] = useState(false);
    const prevCountRef            = useRef(0);

    const {
        notifications,
        unreadCount,
        isConnected,
        isLoading,
        markAllAsRead,
        handleNotificationClick,
    } = useNotificationContext();

    // Shake the bell when a new notification arrives
    useEffect(() => {
        if (unreadCount > prevCountRef.current) {
            setDidShake(true);
            const t = setTimeout(() => setDidShake(false), 600);
            prevCountRef.current = unreadCount;
            return () => clearTimeout(t);
        }
        prevCountRef.current = unreadCount;
    }, [unreadCount]);

    const toggle = () => setIsOpen(prev => !prev);
    const close  = () => setIsOpen(false);

    const handleItemClick = (notification) => {
        close();
        handleNotificationClick(notification);
    };

    const badgeCount = unreadCount > 99 ? 99 : unreadCount;

    return (
        <div className="nm-bell">
            {isOpen && <div className="nm-bell__overlay" onClick={close} />}

            <button
                className={`nm-bell__btn${isOpen ? ' nm-bell__btn--open' : ''}`}
                onClick={toggle}
                aria-label={`Notifications${unreadCount > 0 ? `, ${unreadCount} unread` : ''}`}
                aria-expanded={isOpen}
            >
                <span className={`nm-bell__icon${didShake ? ' nm-bell__icon--shake' : ''}`}>
                    <svg width="17" height="17" viewBox="0 0 24 24" fill="none"
                        stroke="currentColor" strokeWidth="2"
                        strokeLinecap="round" strokeLinejoin="round">
                        <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/>
                        <path d="M13.73 21a2 2 0 0 1-3.46 0"/>
                    </svg>
                </span>

                {unreadCount > 0 && (
                    <span className="nm-bell__badge">{badgeCount}</span>
                )}

                {isConnected && (
                    <span className="nm-bell__live" title="Real-time connected" />
                )}
            </button>

            {isOpen && (
                <NotificationDropdown
                    notifications={notifications.slice(0, PREVIEW_COUNT)}
                    unreadCount={unreadCount}
                    isLoading={isLoading}
                    onMarkAllRead={markAllAsRead}
                    onItemClick={handleItemClick}
                />
            )}
        </div>
    );
};

export default NotificationBell;