import '../styles/notification-dropdown.css';
import NotificationItem from './NotificationItem';

const NotificationDropdown = ({
    notifications,
    unreadCount,
    isLoading,
    onMarkAsRead,
    onMarkAllRead,
    onItemClick,
}) => {
    return (
        <div className="nm-dd" role="dialog" aria-label="Панел с известия">

            <div className="nm-dd__header">
                <span className="nm-dd__title">Известия</span>
                <button
                    className="nm-dd__mark-all"
                    onClick={onMarkAllRead}
                    disabled={unreadCount === 0}
                >
                    Маркирай всички като прочетени
                </button>
            </div>

            {isLoading ? (
                <>
                    {[1, 2, 3].map(i => (
                        <div key={i} className="nm-dd__skeleton-row">
                            <div className="nm-dd__skeleton-circle" />
                            <div className="nm-dd__skeleton-lines">
                                <div className="nm-dd__skeleton-line" />
                                <div className="nm-dd__skeleton-line nm-dd__skeleton-line--short" />
                            </div>
                        </div>
                    ))}
                </>
            ) : notifications.length === 0 ? (
                <div className="nm-dd__empty">
                    <svg width="32" height="32" viewBox="0 0 24 24" fill="none"
                        stroke="currentColor" strokeWidth="1.5">
                        <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
                        <path d="M13.73 21a2 2 0 0 1-3.46 0" />
                    </svg>
                    <p>Няма известия</p>
                </div>
            ) : (
                <ul className="nm-dd__list">
                    {notifications.map(n => (
                        <NotificationItem
                            key={n.id}
                            notification={n}
                            onClick={onItemClick}
                        />
                    ))}
                </ul>
            )}

            <div className="nm-dd__footer">
                <a href="/notifications" className="nm-dd__view-all">
                    Всички известия
                </a>
            </div>
        </div>
    );
};

export default NotificationDropdown;