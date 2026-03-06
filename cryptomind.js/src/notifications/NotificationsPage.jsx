import { useState } from 'react';
import '../styles/notifications-page.css';

import { NotificationType } from './UseNotifications';
import { useNotificationContext } from '../App.jsx';
import NotificationItem from './NotificationItem';

const FILTERS = [
    { key: 'all', label: 'Всички' },
    { key: 'unread', label: 'Непрочетени' },
    { key: 'cipher', label: 'Шифри' },
    { key: 'answer', label: 'Отговори' },
    { key: 'badge', label: 'Значки' },
];

const CIPHER_TYPES = new Set([
    NotificationType.CipherApproved,
    NotificationType.CipherRejected,
    NotificationType.CipherDeleted,
    NotificationType.CipherRestored,
    NotificationType.CipherUpdated
]);

const ANSWER_TYPES = new Set([
    NotificationType.AnswerApproved,
    NotificationType.AnswerRejected,
    NotificationType.AnswerCipherDeleted,
    NotificationType.AnswerCipherRestored,
]);

function matchesFilter(n, key) {
    if (key === 'all') return true;
    if (key === 'unread') return !n.isRead;
    if (key === 'cipher') return CIPHER_TYPES.has(n.type);
    if (key === 'answer') return ANSWER_TYPES.has(n.type);
    if (key === 'badge') return n.type === NotificationType.BadgeEarned;
    return true;
}

function parseSecondsFromTimespan(ts) {
    if (!ts) return Infinity;
    const withDays = ts.match(/^(\d+)\.(\d{2}):(\d{2}):(\d{2})/);
    const timeOnly = ts.match(/^(\d{2}):(\d{2}):(\d{2})/);
    if (withDays) {
        const [, d, h, m, s] = withDays.map(Number);
        return d * 86400 + h * 3600 + m * 60 + s;
    }
    if (timeOnly) {
        const [, h, m, s] = timeOnly.map(Number);
        return h * 3600 + m * 60 + s;
    }
    return Infinity;
}

function groupNotifications(notifications) {
    const TODAY_SEC = 24 * 3600;
    const WEEK_SEC = 7 * TODAY_SEC;

    const groups = { 'Днес': [], 'Тази седмица': [], 'По-рано': [] };

    for (const n of notifications) {
        const sec = parseSecondsFromTimespan(n.createdSince);
        if (sec < TODAY_SEC) groups['Днес'].push(n);
        else if (sec < WEEK_SEC) groups['Тази седмица'].push(n);
        else groups['По-рано'].push(n);
    }

    return Object.entries(groups)
        .filter(([, items]) => items.length > 0)
        .map(([label, items]) => ({ label, items }));
}

const Skeleton = () => (
    <>
        {[1, 2].map(g => (
            <div key={g} className="np-skeleton-card">
                {[1, 2, 3].map(r => (
                    <div key={r} className="np-skeleton-row">
                        <div className="np-skeleton-avatar" />
                        <div className="np-skeleton-lines">
                            <div className="np-skeleton-line" />
                            <div className="np-skeleton-line np-skeleton-line--short" />
                        </div>
                    </div>
                ))}
            </div>
        ))}
    </>
);

const NotificationsPage = () => {
    const [activeFilter, setActiveFilter] = useState('all');
    const {
        notifications,
        unreadCount,
        isLoading,
        markAllAsRead,
        handleNotificationClick,
    } = useNotificationContext();

    const filtered = notifications.filter(n => matchesFilter(n, activeFilter));
    const groups = groupNotifications(filtered);

    return (
        <div className="np-page">

            <div className="np-header">
                <div className="np-header__inner">
                    <p className="np-header__eyebrow">Вашата активност</p>
                    <h1 className="np-header__title">Известия</h1>
                    <p className="np-header__sub">
                        Одобрения, отхвърляния на шифри, отговори и спечелени значки.
                    </p>
                </div>
            </div>

            <div className="np-content">
                <div className="np-toolbar">
                    <div className="np-filters" role="tablist">
                        {FILTERS.map(f => (
                            <button
                                key={f.key}
                                className={`np-filter-btn${activeFilter === f.key ? ' np-filter-btn--active' : ''}`}
                                onClick={() => setActiveFilter(f.key)}
                                role="tab"
                                aria-selected={activeFilter === f.key}
                            >
                                {f.label}
                            </button>
                        ))}
                    </div>

                    <button
                        className="np-mark-all"
                        onClick={markAllAsRead}
                        disabled={unreadCount === 0}
                    >
                        Маркирай всички като прочетени
                    </button>
                </div>

                {!isLoading && unreadCount > 0 && (
                    <p className="np-summary">
                        <span className="np-summary__n">{unreadCount}</span>
                        {' '}непрочетен{unreadCount !== 1 ? 'и известия' : 'о известие'}
                    </p>
                )}

                {isLoading ? (
                    <Skeleton />
                ) : filtered.length === 0 ? (
                    <div className="np-empty">
                        <div className="np-empty__icon">
                            <svg width="26" height="26" viewBox="0 0 24 24" fill="none"
                                stroke="currentColor" strokeWidth="1.5">
                                <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
                                <path d="M13.73 21a2 2 0 0 1-3.46 0" />
                            </svg>
                        </div>
                        <h2 className="np-empty__title">Няма нищо тук</h2>
                        <p className="np-empty__text">
                            {activeFilter === 'unread'
                                ? 'Всичко е прочетено.'
                                : 'Няма известия в тази категория.'}
                        </p>
                    </div>
                ) : (
                    groups.map(group => (
                        <div key={group.label}>
                            <p className="np-group-label">{group.label}</p>
                            <div className="np-card">
                                <ul className="np-list">
                                    {group.items.map(n => (
                                        <NotificationItem
                                            key={n.id}
                                            notification={n}
                                            onClick={handleNotificationClick}
                                        />
                                    ))}
                                </ul>
                            </div>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
};

export default NotificationsPage;