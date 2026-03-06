import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthorizationContext } from './App.jsx';

export default function RequireNotBanned({ mustNotBeBanned = false, children }) {
    const { state } = useContext(AuthorizationContext);
    if (state.isBanned && mustNotBeBanned) {
        return <Navigate to="/banned" replace />;
    }
    else if (!mustNotBeBanned && !state.isBanned) {
        return <Navigate to="/" replace />;
    }
    else {
        return children;
    }
}