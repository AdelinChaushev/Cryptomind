import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthorizationContext } from './App.jsx';

export default function RequireAuth({ allowedRoles = [], mustNotBeLogged = false,children }) {
  const { state } = useContext(AuthorizationContext);
  const { isLoggedIn, roles } = state;
 console.log("isLoggedIn:", isLoggedIn, "roles:", roles, "allowedRoles:", allowedRoles);
  if (mustNotBeLogged && isLoggedIn) {
    return <Navigate to="/" replace />;

  }
  else if (mustNotBeLogged && !isLoggedIn && allowedRoles.length === 0) {
    return children;
  }
  if (!isLoggedIn) {
   
    return <Navigate to="/login" replace />;
  }
  if (allowedRoles.length === 0) {
    return children;
  }
  
  const hasAccess = roles.some(role => allowedRoles.includes(role));
  return hasAccess ? children : <h1>403 - Unauthorized</h1>;
}