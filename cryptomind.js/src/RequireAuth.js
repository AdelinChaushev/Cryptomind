import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthorizationContext } from './App.js';

export default function RequireAuth({ allowedRoles = [], children }) {
  const { isLoggedIn, roles } = useContext(AuthorizationContext);

  if (!isLoggedIn) {
    return <Navigate to="/login" replace />;
  }
  if (allowedRoles.length === 0) {
    return children;
  }

  const hasAccess = roles.some(role => allowedRoles.includes(role));
  return hasAccess ? children : <h1>403 - Unauthorized</h1>;
}