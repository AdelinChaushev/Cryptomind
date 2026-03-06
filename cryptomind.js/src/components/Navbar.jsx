import React, { useState, useContext } from 'react';
import { Link } from 'react-router-dom';
import { AuthorizationContext } from '../App.jsx'; 
import NotificationBell from '../notifications/NotificationBell.jsx';
import useLogout from "./Logout.jsx"
import "../styles/navbar.css"

const Navbar = () => {
    const [menuOpen, setMenuOpen] = useState(false);
    const { state } = useContext(AuthorizationContext);
    const onLogout = useLogout();

    const closeMenu = () => setMenuOpen(false);

    return (
        <nav className="navbar">
            <div className="navbar-container">
                <div className="logo">
                    <img src="/logo.png" alt="" />
                </div>

                <ul className={`nav-links ${menuOpen ? 'active' : ''}`}>
                    {!state.isLoggedIn ? (
                        <li><Link to="/" onClick={closeMenu}>Начало</Link></li>
                    ) : (
                        <>
                            <li><Link to="/" onClick={closeMenu}>Преглед</Link></li>
                            <li><Link to="/submit" onClick={closeMenu}>Предложи</Link></li>
                            <li><Link to="/my_submissions" onClick={closeMenu}>Моите предложения</Link></li>
                        </>
                    )}
                    <li><Link to="/leaderboard" onClick={closeMenu}>Класация</Link></li>
                    <li><Link to="/cipher-library" onClick={closeMenu}>Научи за шифрите</Link></li>
                    <li><Link to="/cipher-tool" onClick={closeMenu}>Тествай шифри</Link></li>
                    <li><Link to="/about" onClick={closeMenu}>За нас</Link></li>

                  
                    <li className="nav-mobile-auth">
                        {state.isLoggedIn ? (
                            <>
                                <Link to="/notifications" className="nav-profile-icon" onClick={closeMenu}>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9"/>
                                        <path d="M13.73 21a2 2 0 0 1-3.46 0"/>
                                    </svg>
                                    Известия
                                </Link>
                                <Link to="/account-info" className="nav-profile-icon" onClick={closeMenu}>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <circle cx="12" cy="8" r="4"/>
                                        <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/>
                                    </svg>
                                    Профил
                                </Link>
                                <button className="btn btn-secondary" onClick={() => { onLogout(); closeMenu(); }}>
                                    Изход
                                </button>
                            </>
                        ) : (
                            <>
                                <Link to="/login" className="btn btn-secondary" onClick={closeMenu}>Вход</Link>
                                <Link to="/register" className="btn btn-primary" onClick={closeMenu}>Започни</Link>
                            </>
                        )}
                    </li>
                </ul>

               
                <div className="nav-desktop-auth">
                    {state.isLoggedIn ? (
                        <>
                            <NotificationBell />
                            <Link to="/account-info" className="nav-profile-icon">
                                <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <circle cx="12" cy="8" r="4"/>
                                    <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7"/>
                                </svg>
                            </Link>
                            <button className="btn btn-secondary" onClick={onLogout}>Изход</button>
                        </>
                    ) : (
                        <div className="nav-buttons">
                            <Link to="/login" className="btn btn-secondary">Вход</Link>
                            <Link to="/register" className="btn btn-primary">Започни</Link>
                        </div>
                    )}
                </div>

               
                <button
                    className={`hamburger ${menuOpen ? 'open' : ''}`}
                    onClick={() => setMenuOpen(prev => !prev)}
                    aria-label="Toggle menu"
                >
                    <span />
                    <span />
                    <span />
                </button>
            </div>

         
            {menuOpen && <div className="nav-overlay" onClick={closeMenu} />}
        </nav>
    );
};

export default Navbar;