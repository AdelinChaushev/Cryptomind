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
                   
                   { !state.isLoggedIn ? (<li><Link to="/">Начало</Link></li>) :
                    (<><li><Link to="/">Преглед</Link></li>
                    <li><Link to="/submit">Предложи</Link></li>
                    <li><Link to="/my_submissions">Моите предложения</Link></li>
                    </>)
                   }
                    <li><Link to="/leaderboard">Класация</Link></li>
                    <li><Link to="/cipher-library">Научи за шифрите</Link></li>
                    <li><Link to="/cipher-tool">Тествай шифри</Link></li>
                    <li><Link to="/about">За нас</Link></li>

                </ul>
                {Array.isArray(state.roles) && state.roles.includes('Admin') && 
                (<><ul className="nav-links">
                    <li><Link to="/admin">Административен панел</Link></li>
                </ul></>)}
                {state.isLoggedIn ?(
                    <>
                      <NotificationBell />
                      <button className="btn btn-secondary" onClick={onLogout}>Изход</button>
                    </>
                )
                    :               
                (<div className="nav-buttons">
                    <Link to="/login" className="btn btn-secondary">Вход</Link>
                    <Link to="/register" className="btn btn-primary">Започни</Link>
                </div>)}
            </div>
        </nav>
    );
};

export default Navbar;