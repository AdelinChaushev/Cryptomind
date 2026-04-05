import React, { useEffect } from 'react';
import { NotificationContext } from './App'; 
import { useNotifications } from './notifications/useNotifications';
import ToastContainer from './notifications/ToastContainer';

export const NotificationProvider = ({ children, isLoggedIn }) => {
    const notifications = useNotifications(isLoggedIn);
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