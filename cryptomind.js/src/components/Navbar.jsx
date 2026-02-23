import React from 'react';
import { Link } from 'react-router-dom';
import { useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { AuthorizationContext } from '../App.jsx'; 
import NotificationBell from '../notifications/NotificationBell.jsx';

const Navbar = () => {
    
   const navigate = useNavigate();
   const {state , setState} = useContext(AuthorizationContext);
//    console.log("Navbar render — full state:", state);
//    console.log("isLoggedIn:", state.isLoggedIn);
//    console.log("roles raw:", state.roles);
//    console.log("is array?", Array.isArray(state.roles));
//    console.log("includes Admin?", state.roles?.includes('Admin'));
//    console.log("includes 'admin' lowercase?", state.roles?.includes('admin'));
//    console.log("roles as JSON:", JSON.stringify(state.roles));
   const onLogout = () => {
    axios.post('/api/auth/logout')
      .then(response => {
        console.log("Logged out successfully");
        setState({ isLoggedIn: false, roles: [] }); 
        navigate('/') 
      })
      .catch(error => {
        console.error("Error logging out:", error);
      });
      console.log(state)
      console.log('Navbar → isLoggedIn:', state.isLoggedIn, 'roles:', state.roles);
   }
    return (
        <nav className="navbar">
            <div className="navbar-container">
                <div className="logo"><img src="/logo.png" alt="" /></div>
                <ul className="nav-links">
                   
                   { !state.isLoggedIn ? (<li><Link to="/">Home</Link></li>) :
                    (<><li><Link to="/">Browse</Link></li>
                    <li><Link to="/submit">Submit</Link></li>
                   <li><Link to="/my_submissions"> My Submissions</Link></li>
                  
                   </>)
                   }
                    <li><Link to="/leaderboard">Leaderboard</Link></li>
                    <li><Link to="/cipher-library">Learn about ciphers</Link></li>
                    <li><Link to="/cipher-tool">Test ciphers</Link></li>
                    <li><Link to="/about">About</Link></li>
                    {/* { state.roles <li></li>} */}

                </ul>
                {Array.isArray(state.roles) && state.roles.includes('Admin') && 
                (<><ul className="nav-links">
                    <li><Link to="/admin">Admin Dashboard</Link></li>
                </ul></>)}
                {state.isLoggedIn ?(
                    <>
                      <NotificationBell />
                    <button className="btn btn-secondary" onClick={onLogout}>Logout</button>
                    </>
                )
                    :               
                (<div className="nav-buttons">
                    <Link to="/login" className="btn btn-secondary">Login</Link>
                    <Link to="/register" className="btn btn-primary">Get Started</Link>
                </div>)}
            </div>
        </nav>
    );
};

export default Navbar;
