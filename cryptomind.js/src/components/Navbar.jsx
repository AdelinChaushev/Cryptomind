import React, { useState, useContext, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { AuthorizationContext } from '../App.jsx';
import NotificationBell from '../notifications/NotificationBell.jsx';
import useLogout from './Logout.jsx';
import '../styles/navbar.css';

const Navbar = () => {
    const [menuOpen, setMenuOpen] = useState(false);
    const { state } = useContext(AuthorizationContext);
    const onLogout = useLogout();

    const closeMenu = () => setMenuOpen(false);

    useEffect(() => {
        document.body.style.overflow = menuOpen ? 'hidden' : '';
        return () => { document.body.style.overflow = ''; };
    }, [menuOpen]);

    const publicLinks = [
        { to: '/',               label: 'Начало' },
        { to: '/leaderboard',    label: 'Класация' },
        { to: '/cipher-library', label: 'Научи за шифрите' },
        { to: '/cipher-tool',    label: 'Тествай шифри' },
        { to: '/about',          label: 'За нас' },
    ];

    const authLinks = [
        { to: '/',                label: 'Преглед' },
        { to: '/submit',          label: 'Предложи' },
        { to: '/my_submissions',  label: 'Моите предложения' },
        { to: '/race-room',       label: 'Състезание' },
        { to: '/leaderboard',     label: 'Класация' },
        { to: '/cipher-library',  label: 'Научи за шифрите' },
        { to: '/cipher-tool',     label: 'Тествай шифри' },
        { to: '/about',           label: 'За нас' },
    ];

    const links = state.isLoggedIn ? authLinks : publicLinks;

    return (
        <>
            <nav className="navbar">
                <div className="navbar-container">
                    <Link to="/" className="nav-logo-link" onClick={closeMenu}>
                        <img src="/logo.png" alt="Cryptomind" className="nav-logo-img" />
                        <span className="nav-brand-name">Cryptomind</span>
                    </Link>

                    <ul className="nav-links">
                        {links.map((link) => (
                            <li key={link.to}>
                                <Link to={link.to}>{link.label}</Link>
                            </li>
                        ))}
                    </ul>

                    <div className="nav-desktop-auth">
                        {state.isLoggedIn ? (
                            <>
                                <NotificationBell />
                                <Link to="/account-info" className="nav-icon-btn" aria-label="Профил">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.75" strokeLinecap="round" strokeLinejoin="round">
                                        <circle cx="12" cy="8" r="4" />
                                        <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" />
                                    </svg>
                                </Link>
                                <button className="nav-btn-secondary" onClick={onLogout}>Изход</button>
                            </>
                        ) : (
                            <div className="nav-auth-buttons">
                                <Link to="/login" className="nav-btn-ghost">Вход</Link>
                                <Link to="/register" className="nav-btn-primary">Започни</Link>
                            </div>
                        )}
                    </div>

                    <button
                        className={`hamburger ${menuOpen ? 'open' : ''}`}
                        onClick={() => setMenuOpen((p) => !p)}
                        aria-label={menuOpen ? 'Затвори меню' : 'Отвори меню'}
                        aria-expanded={menuOpen}
                    >
                        <span /><span /><span />
                    </button>
                </div>
            </nav>

            <div
                className={`nav-mobile-overlay${menuOpen ? ' nav-mobile-overlay--open' : ''}`}
                aria-hidden={!menuOpen}
            >
                <div className="nav-mobile-inner">
                    <div className="nav-mobile-header">
                        <div className="nav-mobile-logo">
                            <img src="/logo.png" alt="Cryptomind" />
                            <span className="nav-mobile-logo-name">Cryptomind</span>
                        </div>
                        <button className="nav-mobile-close" onClick={closeMenu} aria-label="Затвори меню">
                            <span /><span />
                        </button>
                    </div>

                    <ul className="nav-mobile-links">
                        {links.map((link, i) => (
                            <li className="nav-mobile-item" key={link.to}>
                                <Link
                                    to={link.to}
                                    onClick={closeMenu}
                                    style={{ transitionDelay: menuOpen ? `${i * 0.05 + 0.1}s` : '0s' }}
                                >
                                    {link.label}
                                </Link>
                            </li>
                        ))}
                    </ul>

                    <div className="nav-mobile-auth">
                        {state.isLoggedIn ? (
                            <>
                                <Link to="/notifications" className="nav-mobile-auth-link" onClick={closeMenu}>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
                                        <path d="M13.73 21a2 2 0 0 1-3.46 0" />
                                    </svg>
                                    Известия
                                </Link>
                                <Link to="/account-info" className="nav-mobile-auth-link" onClick={closeMenu}>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <circle cx="12" cy="8" r="4" />
                                        <path d="M4 20c0-4 3.6-7 8-7s8 3 8 7" />
                                    </svg>
                                    Профил
                                </Link>
                                <button
                                    className="nav-btn-secondary nav-mobile-full"
                                    onClick={() => { onLogout(); closeMenu(); }}
                                >
                                    Изход
                                </button>
                            </>
                        ) : (
                            <>
                                <Link to="/login" className="nav-btn-ghost nav-mobile-full" onClick={closeMenu}>Вход</Link>
                                <Link to="/register" className="nav-btn-primary nav-mobile-full" onClick={closeMenu}>Започни</Link>
                            </>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default Navbar;
