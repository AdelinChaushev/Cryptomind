import React from 'react';

function ProfileCard({ user, onDeactivate, deactivating }) {
    const getInitial = (username) => {
        if (!username) return '?';
        return username.charAt(0).toUpperCase();
    };

    const formatDate = (dateStr) => {
        const date = new Date(dateStr);
        if (date.getFullYear() <= 1) return 'N/A';
        return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
    };

    const isAdmin = user.roles?.includes('Admin');

    return (
        <aside className="profile-card">
            <div className="profile-avatar-wrap">
                <div className="profile-avatar">
                    {getInitial(user.username)}
                </div>
                <div className="profile-name">{user.username}</div>
                <div className={`profile-role-badge ${isAdmin ? 'admin' : 'user'}`}>
                    <span className="profile-role-dot" />
                    {isAdmin ? 'Admin' : 'User'}
                </div>
            </div>

            <div className="profile-meta">
                <div className="meta-row">
                    <span className="meta-icon">✉</span>
                    <div className="meta-content">
                        <div className="meta-label">Email</div>
                        <div className="meta-value">{user.email}</div>
                    </div>
                </div>
                <div className="meta-row">
                    <span className="meta-icon">📅</span>
                    <div className="meta-content">
                        <div className="meta-label">Member Since</div>
                        <div className="meta-value">{formatDate(user.registeredAt)}</div>
                    </div>
                </div>
                <div className="meta-row">
                    <span className="meta-icon">🔐</span>
                    <div className="meta-content">
                        <div className="meta-label">Roles</div>
                        <div className="meta-value">{user.roles?.join(', ')}</div>
                    </div>
                </div>
            </div>

            <button
                className="btn-deactivate"
                onClick={onDeactivate}
                disabled={deactivating}
            >
                ⚠ {deactivating ? 'Processing...' : 'Deactivate Account'}
            </button>
        </aside>
    );
}

export default ProfileCard;
