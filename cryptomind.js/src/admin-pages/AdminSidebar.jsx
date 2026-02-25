import React from 'react';
import { Link } from 'react-router-dom';

const AdminSidebar = ({ activePage }) => {
    const navItems = [
        {
            section: 'Общ преглед',
            links: [
                { id: 'dashboard', label: 'Табло', href: '/admin', icon: <GridIcon /> }
            ]
        },
        {
            section: 'Шифри',
            links: [
                { id: 'pending-ciphers', label: 'Изчакващи предложения', href: '/admin/pending-ciphers', icon: <ClockIcon />, badge: true },
                { id: 'approved-ciphers', label: 'Управление на шифри', href: '/admin/manage-ciphers', icon: <LockIcon /> },
                { id: 'deleted-ciphers', label: 'Изтрити шифри', href: '/admin/deleted-ciphers', icon: <TrashIcon /> }
            ]
        },
        {
            section: 'Отговори',
            links: [
                { id: 'pending-answers', label: 'Изчакващи отговори', href: '/admin/pending-answers', icon: <ChatIcon />, badge: true },
            ]
        },
        {
            section: 'Потребители',
            links: [
                { id: 'users', label: 'Управление на потребители', href: '/admin/users', icon: <UsersIcon /> }
            ]
        }
    ];

    return (
        <aside className="admin-sidebar">
            <div className="sidebar-logo">
                <div className="sidebar-logo-text">
                    <span>[</span>CRYPTOMIND<span>]</span>
                </div>
                <div className="sidebar-logo-badge">АДМИН ПАНЕЛ</div>
            </div>

            {navItems.map((group) => (
                <div className="sidebar-section" key={group.section}>
                    <div className="sidebar-section-label">{group.section}</div>
                    <nav className="sidebar-nav">
                        {group.links.map((link) => (
                            <Link
                                key={link.id}
                                to={link.href}
                                className={`sidebar-link${activePage === link.id ? ' active' : ''}`}
                            >
                                <span className="link-icon">{link.icon}</span>
                                {link.label}
                                {link.badge && (
                                    <span className="sidebar-badge" id={`badge-${link.id}`}></span>
                                )}
                            </Link>
                        ))}
                    </nav>
                </div>
            ))}

            <div className="sidebar-footer">
                <div className="sidebar-user">
                    <div className="sidebar-user-info">
                        <span className="sidebar-user-name">admin@cryptomind.com</span>
                        <span className="sidebar-user-role">АДМИН</span>
                    </div>
                </div>
                {
                    <Link to="/logout" className="sidebar-link">
                        <span className="link-icon"><LogoutIcon /></span>
                        Изход
                    </Link>
                }
            </div>
        </aside>
    );
};

/* ─── Икони (SVG) ─── */
const GridIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <rect x="1" y="1" width="6" height="6" rx="1"/><rect x="9" y="1" width="6" height="6" rx="1"/>
        <rect x="1" y="9" width="6" height="6" rx="1"/><rect x="9" y="9" width="6" height="6" rx="1"/>
    </svg>
);

const ClockIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <circle cx="8" cy="8" r="6.5"/><path d="M8 4.5V8l2.5 2"/>
    </svg>
);

const LockIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <rect x="2.5" y="7" width="11" height="8" rx="1.5"/>
        <path d="M5 7V5a3 3 0 0 1 6 0v2"/>
    </svg>
);

const ChatIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <path d="M14 3H2a1 1 0 0 0-1 1v7a1 1 0 0 0 1 1h2v2.5L7 12h7a1 1 0 0 0 1-1V4a1 1 0 0 0-1-1z"/>
    </svg>
);

const UsersIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <circle cx="6" cy="5" r="2.5"/><path d="M1 13c0-2.76 2.24-5 5-5s5 2.24 5 5"/>
        <path d="M11 3a2.5 2.5 0 0 1 0 5M15 13a4 4 0 0 0-4-4"/>
    </svg>
);

const HomeIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <path d="M1 6.5L8 1l7 5.5V15H1V6.5z"/><rect x="5.5" y="10" width="5" height="5"/>
    </svg>
);

const LogoutIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <path d="M6 2H2.5A1.5 1.5 0 0 0 1 3.5v9A1.5 1.5 0 0 0 2.5 14H6"/>
        <path d="M11 11l4-3.5L11 4M6 8.5h9"/>
    </svg>
);

const TrashIcon = () => (
    <svg viewBox="0 0 16 16" fill="none" stroke="currentColor" strokeWidth="1.5">
        <path d="M2 4h12M5 4V2h6v2M6 7v5M10 7v5M3 4l1 10h8l1-10"/>
    </svg>
);

export default AdminSidebar;