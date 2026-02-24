import React, { useState, useEffect } from 'react';
import '../styles/account-info.css';

import ProfileCard    from './ProfileCard';
import StatsSection   from './StatsSection';
import BadgesSection  from './BadgesSection';
import DeactivateModal from './DeactivateModal';

const API_BASE = 'http://localhost:5115';

function AccountInfo() {
    const [user, setUser]               = useState(null);
    const [loading, setLoading]         = useState(true);
    const [error, setError]             = useState(null);
    const [showModal, setShowModal]     = useState(false);
    const [deactivating, setDeactivating] = useState(false);

    useEffect(() => {
        const fetchUser = async () => {
            try {
                const res = await fetch(`${API_BASE}/api/user/get-account-info`, {
                    credentials: 'include',
                });

                if (!res.ok) throw new Error(`Error ${res.status}`);

                const data = await res.json();
                setUser(data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchUser();
    }, []);

    const handleDeactivate = async () => {
        setDeactivating(true);
        try {
            const res = await fetch(`${API_BASE}/api/auth/deactivate`, {
                method: 'POST',
                credentials: 'include',
            });

            if (!res.ok) throw new Error(`Error ${res.status}`);

            // Redirect to login or home after deactivation
            window.location.href = '/login';
        } catch (err) {
            alert(`Failed to deactivate account: ${err.message}`);
        } finally {
            setDeactivating(false);
            setShowModal(false);
        }
    };

    if (loading) {
        return (
            <div className="account-page">
                <div className="account-loading">
                    <div className="loading-spinner" />
                    <div className="loading-text">Loading account info...</div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="account-page">
                <div className="account-error">
                    <div className="error-icon">⚠</div>
                    <div className="error-text">Failed to load account: {error}</div>
                </div>
            </div>
        );
    }

    return (
        <div className="account-page">
            <div className="account-page-header">
                <div className="account-breadcrumb">
                    <span>Home</span>
                    <span className="breadcrumb-sep">/</span>
                    <span className="breadcrumb-current">Account</span>
                </div>
                <h1 className="account-page-title">
                    My <span>Account</span>
                </h1>
                <p className="account-page-subtitle">
                    Manage your profile, track your progress, and view earned badges.
                </p>
            </div>

            <div className="account-layout">
                <ProfileCard
                    user={user}
                    onDeactivate={() => setShowModal(true)}
                    deactivating={deactivating}
                />

                <div className="account-right">
                    <StatsSection user={user} />
                    <BadgesSection badges={user.badges} />
                </div>
            </div>

            {showModal && (
                <DeactivateModal
                    onConfirm={handleDeactivate}
                    onCancel={() => setShowModal(false)}
                    loading={deactivating}
                />
            )}
        </div>
    );
}

export default AccountInfo;
