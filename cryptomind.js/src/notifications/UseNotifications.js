import { useState, useEffect, useCallback, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

export const NotificationType = {
    CipherApproved:        0,
    CipherRejected:        1,
    CipherDeleted:         2,
    CipherRestored:        3,
    CipherUpdated:         4,
    AnswerApproved:        5,
    AnswerRejected:        6,
    AnswerCipherDeleted:   7,
    AnswerCipherRestored:  8,
    BadgeEarned:           9,
};

const API_BASE = 'http://localhost:5115/api/notifications';
const HUB_URL  = 'http://localhost:5115/notificationHub';

/*
 * parseCreatedSince
 *
 * The API returns `createdSince` as a .NET TimeSpan string.
 * Formats:
 *   "00:00:14.5678303"    →  14 seconds ago
 *   "07:36:05.4820075"    →  7 hours ago
 *   "1.03:43:27.4695859"  →  1 day ago
 */
export function parseCreatedSince(createdSince) {
    if (!createdSince) return '';

    // Regex to capture: [days].hours:minutes:seconds
    // Supports formats: "5.12:30:00" or "12:30:00"
    const match = createdSince.match(/(?:(\d+)\.)?(\d{1,2}):(\d{2}):(\d{2})/);

    if (!match) return createdSince;

    // Extract parts: group 1 is days (optional), 2 is hours, 3 is minutes, 4 is seconds
    const daysPart = parseInt(match[1] || 0, 10);
    const hoursPart = parseInt(match[2], 10);
    const minutesPart = parseInt(match[3], 10);
    const secondsPart = parseInt(match[4], 10);

    const totalSeconds = (daysPart * 86400) + (hoursPart * 3600) + (minutesPart * 60) + secondsPart;

    // Determine the largest unit for human-readable output
    if (totalSeconds < 60) return 'just now';

    const mins = Math.floor(totalSeconds / 60);
    if (mins < 60) return `${mins}m ago`;

    const hrs = Math.floor(mins / 60);
    if (hrs < 24) return `${hrs}h ago`;

    const days = Math.floor(hrs / 24);
    if (days < 30) return `${days}d ago`;

    const months = Math.floor(days / 30);
    return `${months}mo ago`;
}

function resolveLink(notification) {
    if (notification.link && notification.link.trim() !== '') {
        const raw = notification.link;
        return raw.startsWith('/') ? raw : `/${raw}`;
    }
   
     switch (notification.type) {
        case 0: // CipherApproved
        case 1: // CipherRejected
        case 2: // CipherDeleted
        case 3: // CipherRestored
        case 4: // CipherUpdated
            return '/my-submissions';
        case 5: // AnswerApproved
        case 6: // AnswerRejected
        case 7: // AnswerCipherDeleted
        case 8: // AnswerCipherRestored
            return '/my-submissions';
        case 9: // BadgeEarned
            return '/profile#badges';
        default:
            return null;
    }
}

export function useNotifications() {
    const [notifications, setNotifications] = useState([]);
    const [unreadCount, setUnreadCount]     = useState(0);
    const [isConnected, setIsConnected]     = useState(false);
    const [isLoading, setIsLoading]         = useState(true);
    const [toasts, setToasts]               = useState([]);
    const connectionRef                     = useRef(null);

    // ── Toast dismiss ──────────────────────────────────────────
    const dismissToast = useCallback((toastId) => {
        setToasts(prev =>
            prev.map(t => t.id === toastId ? { ...t, isLeaving: true } : t)
        );
        setTimeout(() => {
            setToasts(prev => prev.filter(t => t.id !== toastId));
        }, 220);
    }, []);

 
    const authHeaders = () => ({ 'Content-Type': 'application/json' });
    const didStartRef = useRef(false);
    // ── Fetch notification history ─────────────────────────────
    const fetchNotifications = useCallback(async () => {
        try {
            const res = await fetch(API_BASE, {
                headers: authHeaders(),
                credentials: 'include', // sends the HttpOnly cookie automatically
            });
            if (!res.ok) {
                console.error('[Notifications] GET returned', res.status);
                return;
            }
            const body = await res.json();
            setNotifications(body.notifications ?? []);
            setUnreadCount(body.unreadCount ?? 0);
        } catch (err) {
            console.error('[Notifications] Fetch failed:', err);
        } finally {
            setIsLoading(false);
        }
    }, []);

  
    
    useEffect(() => {
        if (didStartRef.current) return; // StrictMode guard
       didStartRef.current = true;
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL, {
                withCredentials: true, // sends HttpOnly cookie on negotiate
                transport:
                    signalR.HttpTransportType.WebSockets       |
                    signalR.HttpTransportType.ServerSentEvents |
                    signalR.HttpTransportType.LongPolling,
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        connection.on('ReceiveNotification', (incoming) => {
            console.log('[SignalR] ReceiveNotification:', incoming);
            setNotifications(prev => [incoming, ...prev]);
            setUnreadCount(prev => prev + 1);
            setToasts(prev => [
                ...prev,
                {
                    id: `toast-${incoming.id}-${Date.now()}`,
                    notification: incoming,
                    isLeaving: false,
                },
            ]);
        });

        connection.onreconnecting(() => {
            console.warn('[SignalR] Reconnecting...');
            setIsConnected(false);
        });

        connection.onreconnected(() => {
            console.log('[SignalR] Reconnected');
            setIsConnected(true);
            fetchNotifications();
        });

        connection.onclose((err) => {
            console.warn('[SignalR] Closed', err?.message ?? '');
            setIsConnected(false);
        });

        const start = async () => {
            try {
                await connection.start();
                console.log('[SignalR] Connected. State:', connection.state);
                setIsConnected(true);
            } catch (err) {
                console.error('[SignalR] start() failed:', err);
                setTimeout(start, 5000);
            }
        };

        start();
        connectionRef.current = connection;

        return () => connection.stop();
    }, []);

    useEffect(() => {
        fetchNotifications();
    }, [fetchNotifications]);

    // ── Mark one as read ───────────────────────────────────────
    const markAsRead = useCallback(async (notificationId) => {
        setNotifications(prev =>
            prev.map(n => n.id === notificationId ? { ...n, isRead: true } : n)
        );
        setUnreadCount(prev => Math.max(0, prev - 1));
        console.log(notificationId)
        try {

            const res = await fetch(`${API_BASE}/mark-as-read/${notificationId}`, {
                method: 'PUT',
                headers: authHeaders(),
                credentials: 'include',
               
            });
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
        } catch (err) {
            console.error('[Notifications] markAsRead failed, rolling back:', err);
            setNotifications(prev =>
                prev.map(n => n.id === notificationId ? { ...n, isRead: false } : n)
            );
            setUnreadCount(prev => prev + 1);
        }
    }, []);

    // ── Mark all as read ───────────────────────────────────────
    const markAllAsRead = useCallback(async () => {
        const unreadIds = notifications.filter(n => !n.isRead).map(n => n.id);
        if (unreadIds.length === 0) return;

        const snapshot = notifications;
        setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
        setUnreadCount(0);

        try {
            const res = await fetch(`${API_BASE}/mark-as-read`, {
                method: 'PUT',
                headers: authHeaders(),
                credentials: 'include',
            });
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
        } catch (err) {
            console.error('[Notifications] markAllAsRead failed, rolling back:', err);
            setNotifications(snapshot);
            setUnreadCount(unreadIds.length);
        }
    }, [notifications]);

    // ── Click: mark read + navigate ────────────────────────────
    const handleNotificationClick = useCallback(async (notification) => {
        if (!notification.isRead) {
            await markAsRead(notification.id);
        }
        const link = resolveLink(notification);
        if (link) window.location.href = link;
    }, [markAsRead]);

    return {
        notifications,
        unreadCount,
        isConnected,
        isLoading,
        markAsRead,
        markAllAsRead,
        handleNotificationClick,
        toasts,
        dismissToast,
        refetch: fetchNotifications,
    };
}