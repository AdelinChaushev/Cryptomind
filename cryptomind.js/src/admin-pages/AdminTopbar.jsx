import React from 'react';

const AdminTopbar = ({ breadcrumbs = [], children }) => {
    return (
        <header className="admin-topbar">
            <div className="topbar-breadcrumb">
                <a href="/admin" className="sidebar-link" style={{ padding: 0, color: 'var(--text-dim)' }}>
                    Admin
                </a>
                {breadcrumbs.map((crumb, i) => (
                    <React.Fragment key={i}>
                        <span className="sep">/</span>
                        {crumb.href ? (
                            <a href={crumb.href} style={{ color: 'var(--text-dim)' }}>{crumb.label}</a>
                        ) : (
                            <span className="current">{crumb.label}</span>
                        )}
                    </React.Fragment>
                ))}
            </div>

            <div className="topbar-actions">
                {children}
            </div>
        </header>
    );
};

export default AdminTopbar;
