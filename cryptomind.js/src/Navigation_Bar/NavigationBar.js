import { Link } from 'react-router-dom';
import './styles/NavigationBar.css';

import { useContext } from 'react';
import { AuthorizationContext } from '../App.js'; 
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
export default function NavigationBar(){
   const navigate = useNavigate();
  const context = useContext(AuthorizationContext);
   const onLogout = () => {
    axios.post('/api/User/logout')
      .then(response => {
        console.log("Logged out successfully");
        context.isLoggedIn = false;
        navigate('/') // Update the context state
      })
      .catch(error => {
        console.error("Error logging out:", error);
      });
      

   }

    return(
      <>
      <div className="navbar-left">
    <Link to="/" className="navbar-logo">Cryptomind</Link>
    <div className="navbar-links">
      <Link to="/">Home</Link>    
    </div>
  </div>
  <div className="navbar-auth">

  {!context.isLoggedIn && (
    <div>
      <Link to="/login" className="auth-button">Log In</Link>
      <Link to="/register" className="auth-button register">Register</Link>
    </div>
  )}
   {context.isLoggedIn && (
    <div>
      <Link  to="/" className="auth-button" onClick={onLogout}>Log out</Link>
      <Link  to="/ciphers" className="auth-button">Ciphers</Link>
    </div>
  )}
  
  </div>
</>

    )
    
}