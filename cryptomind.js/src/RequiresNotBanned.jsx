import { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import { AuthorizationContext } from './App.jsx';

export default function RequireNotBanned({ mustNotBeBanned = false,children }) {
    const {state}  = useContext(AuthorizationContext);
     if (state.isBanned && mustNotBeBanned) { 
        console.log("User is not banned, rendering children." + " isBanned: " + state.isBanned + ", mustNotBeBanned: " + mustNotBeBanned);
        return <Navigate to="/banned" replace />;
     }        
    else if(!mustNotBeBanned && !state.isBanned){
        console.log("User is not banned, rendering children." + " isBanned: " + state.isBanned + ", mustNotBeBanned: " + mustNotBeBanned);
        return <Navigate to="/" replace />;
    } 
    else{
        return children;
    }
}