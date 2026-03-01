import React, { useEffect } from 'react';
import { NotificationContext } from './App'; // Ensure this is exported from App.jsx
import { useNotifications } from './notifications/UseNotifications';
import ToastContainer from './notifications/ToastContainer';

export const NotificationProvider = ({ children, isLoggedIn }) => {
    const notifications = useNotifications();

    useEffect(() => {
        if (isLoggedIn) {
            notifications.refetch();
        }
    }, [isLoggedIn, notifications]);

    return (
        <NotificationContext.Provider value={notifications}>
            {children}
           
            <ToastContainer
                toasts={notifications.toasts}
                onDismiss={notifications.dismissToast}
                onNotificationClick={notifications.handleNotificationClick}
            />
        </NotificationContext.Provider>
    );
};